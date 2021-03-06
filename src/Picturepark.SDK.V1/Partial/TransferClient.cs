﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Picturepark.SDK.V1.Contract;
using System.Net.Http;

namespace Picturepark.SDK.V1
{
	public partial class TransferClient
	{
		private static readonly object s_lock = new object();

		private readonly BusinessProcessClient _businessProcessClient;
		private volatile List<string> _fileNameBlacklist;

		public TransferClient(BusinessProcessClient businessProcessClient, IPictureparkClientSettings settings, HttpClient httpClient)
			: this(settings, httpClient)
		{
			_businessProcessClient = businessProcessClient;
		}

		/// <summary>Searches files of a given transfer ID.</summary>
		/// <param name="transferId">The transfer ID.</param>
		/// <param name="limit">The maximum number of search results.</param>
		/// <returns>The result.</returns>
		public async Task<FileTransferSearchResult> SearchFilesByTransferIdAsync(string transferId, int limit = 20)
		{
			var request = new FileTransferSearchRequest()
			{
				Limit = limit,
				SearchString = "*",
				Filter = new TermFilter
				{
					Field = "transferId",
					Term = transferId
				}
			};

			return await SearchFilesAsync(request);
		}

		/// <summary>Uploads multiple files from the filesystem.</summary>
		/// <param name="transferName">The name of the created transfer.</param>
		/// <param name="filePaths">The file paths on the filesystem.</param>
		/// <param name="uploadOptions">The file upload options.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>The created transfer object.</returns>
		public async Task<CreateTransferResult> UploadFilesAsync(string transferName, IEnumerable<string> filePaths, UploadOptions uploadOptions, CancellationToken cancellationToken = default(CancellationToken))
		{
			var filteredFileNames = FilterFilesByBlacklist(filePaths.ToList());
			var result = await CreateAndWaitForCompletionAsync(transferName, filteredFileNames.Select(Path.GetFileName), cancellationToken).ConfigureAwait(false);
			await UploadFilesAsync(result.Transfer, filteredFileNames, uploadOptions, cancellationToken).ConfigureAwait(false);
			return result;
		}

		/// <summary>Uploads multiple files from the filesystem.</summary>
		/// <param name="transfer">The existing transfer object.</param>
		/// <param name="filePaths">The file paths on the filesystem.</param>
		/// <param name="uploadOptions">The file upload options.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>The created transfer object.</returns>
		public async Task UploadFilesAsync(Transfer transfer, IEnumerable<string> filePaths, UploadOptions uploadOptions, CancellationToken cancellationToken = default(CancellationToken))
		{
			uploadOptions = uploadOptions ?? new UploadOptions();

			var exceptions = new List<Exception>();

			// Limits concurrent downloads
			var throttler = new SemaphoreSlim(uploadOptions.ConcurrentUploads);
			var filteredFileNames = FilterFilesByBlacklist(filePaths.ToList());

			// TODO: File by file uploads
			var tasks = filteredFileNames
				.Select(file => Task.Run(async () =>
				{
					try
					{
						await UploadFileAsync(throttler, transfer.Id, file, uploadOptions.ChunkSize, cancellationToken).ConfigureAwait(false);
						uploadOptions.SuccessDelegate?.Invoke(file);
					}
					catch (Exception ex)
					{
						exceptions.Add(ex);
						uploadOptions.ErrorDelegate?.Invoke(ex);
					}
				}));

			await Task.WhenAll(tasks).ConfigureAwait(false);

			if (exceptions.Any())
			{
				throw new AggregateException(exceptions);
			}

			if (uploadOptions.WaitForTransferCompletion)
			{
				await _businessProcessClient.WaitForCompletionAsync(transfer.BusinessProcessId, cancellationToken: cancellationToken).ConfigureAwait(false);
			}
		}

		/// <summary>Transfers the uploaded files and waits for its completions.</summary>
		/// <param name="transfer">The transfer.</param>
		/// <param name="createRequest">The create request.</param>
		/// <param name="timeout">The timeout in ms to wait for completion.</param>
		/// <param name="cancellationToken">The cancellcation token.</param>
		/// <returns>The task.</returns>
		public async Task ImportAndWaitForCompletionAsync(Transfer transfer, FileTransfer2ContentCreateRequest createRequest, TimeSpan? timeout = null, CancellationToken cancellationToken = default(CancellationToken))
		{
			var importedTransfer = await ImportTransferAsync(transfer.Id, createRequest, cancellationToken).ConfigureAwait(false);
			await _businessProcessClient.WaitForCompletionAsync(importedTransfer.BusinessProcessId, timeout, cancellationToken).ConfigureAwait(false);
		}

		/// <summary>Creates a transfer and waits for its completion.</summary>
		/// <param name="request">The create request.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>The transfer.</returns>
		public async Task<CreateTransferResult> CreateAndWaitForCompletionAsync(CreateTransferRequest request, CancellationToken cancellationToken = default(CancellationToken))
		{
			var transfer = await CreateAsync(request, cancellationToken).ConfigureAwait(false);
			await _businessProcessClient.WaitForCompletionAsync(transfer.BusinessProcessId, cancellationToken: cancellationToken).ConfigureAwait(false);

			return new CreateTransferResult(transfer, request.Files);
		}

		/// <summary>Creates a transfer and waits for its completion.</summary>
		/// <param name="transferName">The name of the transfer.</param>
		/// <param name="fileNames">The file names of the transfer.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>The transfer.</returns>
		public async Task<CreateTransferResult> CreateAndWaitForCompletionAsync(string transferName, IEnumerable<string> fileNames, CancellationToken cancellationToken = default(CancellationToken))
		{
			var filteredFileNames = FilterFilesByBlacklist(fileNames);

			var request = new CreateTransferRequest
			{
				Name = string.IsNullOrEmpty(transferName) ? new Random().Next(1000, 9999).ToString() : transferName,
				TransferType = TransferType.FileUpload,
				Files = filteredFileNames.Select(i => new TransferUploadFile
				{
					Identifier = Guid.NewGuid().ToString(),
					FileName = i
				}).ToList()
			};

			var transfer = await CreateAsync(request, cancellationToken).ConfigureAwait(false);
			await _businessProcessClient.WaitForStatesAsync(transfer.BusinessProcessId, new[] { TransferState.Created.ToString() }, null, cancellationToken).ConfigureAwait(false);

			return new CreateTransferResult(transfer, request.Files);
		}

		private async Task UploadFileAsync(SemaphoreSlim throttler, string transferId, string absoluteFilePath, int chunkSize, CancellationToken cancellationToken = default(CancellationToken))
		{
			var fileName = Path.GetFileName(absoluteFilePath);
			var identifier = fileName;

			var fileSize = new FileInfo(absoluteFilePath).Length;
			var relativePath = fileName;
			var totalChunks = (long)Math.Ceiling((decimal)fileSize / chunkSize);

			var uploadTasks = new List<Task>();

			for (var chunkNumber = 1; chunkNumber <= totalChunks; chunkNumber++)
			{
				await throttler.WaitAsync(cancellationToken).ConfigureAwait(false);

				var number = chunkNumber;
				uploadTasks.Add(Task.Run(async () =>
				{
					try
					{
						using (var fileStream = File.OpenRead(absoluteFilePath))
						{
							var currentChunkSize = chunkSize;
							var position = (number - 1) * chunkSize;

							// last chunk may have a different size.
							if (number == totalChunks)
							{
								currentChunkSize = (int)(fileSize - position);
							}

							var buffer = new byte[currentChunkSize];
							fileStream.Position = position;

							await fileStream.ReadAsync(buffer, 0, currentChunkSize, cancellationToken).ConfigureAwait(false);

							using (var memoryStream = new MemoryStream(buffer))
							{
								await UploadFileAsync(
									transferId,
									identifier,
									new FileParameter(memoryStream, fileName),
									relativePath,
									number,
									currentChunkSize,
									fileSize,
									totalChunks,
									cancellationToken).ConfigureAwait(false);
							}
						}
					}
					finally
					{
						throttler.Release();
					}
				}));
			}

			await Task.WhenAll(uploadTasks).ConfigureAwait(false);
		}

		private IEnumerable<string> FilterFilesByBlacklist(IEnumerable<string> files)
		{
			var blacklist = GetCachedBlacklist();
			var filteredFileNames = files.Where(i => !blacklist.Contains(Path.GetFileName(i).ToLowerInvariant()));
			return filteredFileNames;
		}

		private List<string> GetCachedBlacklist()
		{
			if (_fileNameBlacklist != null)
				return _fileNameBlacklist;

			lock (s_lock)
			{
				if (_fileNameBlacklist != null)
					return _fileNameBlacklist;

				var blacklist = Task.Run(async () => await GetBlacklistAsync()).GetAwaiter().GetResult(); // TODO: GetCachedBlacklist: Use async version
				_fileNameBlacklist = blacklist.Items.Select(i => i.Match.ToLowerInvariant()).ToList();
			}

			return _fileNameBlacklist;
		}
	}
}
