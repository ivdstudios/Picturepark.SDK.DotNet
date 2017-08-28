﻿//----------------------
// <auto-generated>
//     Generated using the NSwag toolchain v11.3.3.0 (NJsonSchema v9.4.2.0) (http://NSwag.org)
// </auto-generated>
//----------------------

namespace Picturepark.SDK.V1.Contract
{
    #pragma warning disable // Disable all warnings

    

    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "9.4.2.0")]
    public partial class LiveStreamMessage 
    {
        [Newtonsoft.Json.JsonProperty("Id", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string Id { get; set; }
    
        [Newtonsoft.Json.JsonProperty("Priority", Required = Newtonsoft.Json.Required.Always)]
        public int Priority { get; set; }
    
        [Newtonsoft.Json.JsonProperty("CustomerId", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string CustomerId { get; set; }
    
        [Newtonsoft.Json.JsonProperty("CustomerAlias", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string CustomerAlias { get; set; }
    
        [Newtonsoft.Json.JsonProperty("Timestamp", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required]
        public System.DateTime Timestamp { get; set; }
    
        [Newtonsoft.Json.JsonProperty("Scope", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string Scope { get; set; }
    
        [Newtonsoft.Json.JsonProperty("DocumentChange", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public DocumentChange DocumentChange { get; set; }
    
        [Newtonsoft.Json.JsonProperty("ApplicationEvent", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public ApplicationEvent ApplicationEvent { get; set; }
    
        [Newtonsoft.Json.JsonProperty("SearchRequest", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public SearchRequest SearchRequest { get; set; }
    
        [Newtonsoft.Json.JsonProperty("Retries", Required = Newtonsoft.Json.Required.Always)]
        public int Retries { get; set; }
    
        public string ToJson() 
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(this);
        }
        
        public static LiveStreamMessage FromJson(string data)
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject<LiveStreamMessage>(data);
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "9.4.2.0")]
    public partial class DocumentChange 
    {
        [Newtonsoft.Json.JsonProperty("DocumentName", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string DocumentName { get; set; }
    
        [Newtonsoft.Json.JsonProperty("DocumentId", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string DocumentId { get; set; }
    
        [Newtonsoft.Json.JsonProperty("Version", Required = Newtonsoft.Json.Required.Always)]
        public long Version { get; set; }
    
        [Newtonsoft.Json.JsonProperty("Action", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string Action { get; set; }
    
        [Newtonsoft.Json.JsonProperty("TimeStamp", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required]
        public System.DateTime TimeStamp { get; set; }
    
        public string ToJson() 
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(this);
        }
        
        public static DocumentChange FromJson(string data)
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject<DocumentChange>(data);
        }
    }
    
    [Newtonsoft.Json.JsonConverter(typeof(JsonInheritanceConverter), "kind")]
    [JsonInheritanceAttribute("TransferEvent", typeof(TransferEvent))]
    [JsonInheritanceAttribute("FileTransferEvent", typeof(FileTransferEvent))]
    [JsonInheritanceAttribute("ReindexEvent", typeof(ReindexEvent))]
    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "9.4.2.0")]
    public partial class ApplicationEvent 
    {
        [Newtonsoft.Json.JsonProperty("SilentMode", Required = Newtonsoft.Json.Required.Always)]
        public bool SilentMode { get; set; }
    
        [Newtonsoft.Json.JsonProperty("EventType", Required = Newtonsoft.Json.Required.Always)]
        public ApplicationEventType EventType { get; set; }
    
        [Newtonsoft.Json.JsonProperty("Timestamp", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required]
        public System.DateTime Timestamp { get; set; }
    
        public string ToJson() 
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(this);
        }
        
        public static ApplicationEvent FromJson(string data)
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject<ApplicationEvent>(data);
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "9.4.2.0")]
    public enum ApplicationEventType
    {
        Info = 1,
    
        Transition = 2,
    
    }
    
    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "9.4.2.0")]
    public partial class TransferEvent : ApplicationEvent
    {
        [Newtonsoft.Json.JsonProperty("TransferId", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string TransferId { get; set; }
    
        [Newtonsoft.Json.JsonProperty("State", Required = Newtonsoft.Json.Required.Always)]
        public TransferState State { get; set; }
    
        public string ToJson() 
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(this);
        }
        
        public static TransferEvent FromJson(string data)
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject<TransferEvent>(data);
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "9.4.2.0")]
    public partial class FileTransferEvent : ApplicationEvent
    {
        [Newtonsoft.Json.JsonProperty("TransferId", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string TransferId { get; set; }
    
        [Newtonsoft.Json.JsonProperty("FileTransferId", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string FileTransferId { get; set; }
    
        [Newtonsoft.Json.JsonProperty("State", Required = Newtonsoft.Json.Required.Always)]
        public FileTransferState State { get; set; }
    
        public string ToJson() 
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(this);
        }
        
        public static FileTransferEvent FromJson(string data)
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject<FileTransferEvent>(data);
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "9.4.2.0")]
    public partial class ReindexEvent : ApplicationEvent
    {
        [Newtonsoft.Json.JsonProperty("IndexId", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string IndexId { get; set; }
    
        [Newtonsoft.Json.JsonProperty("State", Required = Newtonsoft.Json.Required.Always)]
        public IndexState State { get; set; }
    
        public string ToJson() 
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(this);
        }
        
        public static ReindexEvent FromJson(string data)
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject<ReindexEvent>(data);
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "9.4.2.0")]
    public enum IndexState
    {
        Draft = 1,
    
        Create = 2,
    
        Inactive = 5,
    
        Active = 9,
    
        CloseInProgress = 10,
    
        CloseCompleted = 11,
    
        CloseFailed = 12,
    
        Closed = 13,
    
        OpenInProgress = 14,
    
        OpenCompleted = 15,
    
        OpenFailed = 16,
    
        SnapshotInProgress = 17,
    
        SnapshotCompleted = 18,
    
        SnapshotFailed = 19,
    
        ReindexInProgress = 20,
    
        ReindexCompleted = 21,
    
        ReindexFailed = 22,
    
        DeactivationInProgress = 26,
    
        DeactivationCompleted = 27,
    
        DeactivationFailed = 28,
    
    }
    
    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "9.4.2.0")]
    public partial class SearchRequest 
    {
        [Newtonsoft.Json.JsonProperty("UserId", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string UserId { get; set; }
    
        [Newtonsoft.Json.JsonProperty("SearchString", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string SearchString { get; set; }
    
        [Newtonsoft.Json.JsonProperty("TimeStamp", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required]
        public System.DateTime TimeStamp { get; set; }
    
        public string ToJson() 
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(this);
        }
        
        public static SearchRequest FromJson(string data)
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject<SearchRequest>(data);
        }
    }
}