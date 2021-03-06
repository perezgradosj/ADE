﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ADE.Processes.SunatDelivery.SunatPRDr {
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(Namespace="http://service.sunat.gob.pe", ConfigurationName="SunatPRDr.billService")]
    public interface billService {
        
        // CODEGEN: Parameter 'status' requires additional schema information that cannot be captured using the parameter mode. The specific attribute is 'System.Xml.Serialization.XmlElementAttribute'.
        [System.ServiceModel.OperationContractAttribute(Action="urn:getStatus", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        [return: System.ServiceModel.MessageParameterAttribute(Name="status")]
        ADE.Processes.SunatDelivery.SunatPRDr.getStatusResponse getStatus(ADE.Processes.SunatDelivery.SunatPRDr.getStatusRequest request);
        
        [System.ServiceModel.OperationContractAttribute(Action="urn:getStatus", ReplyAction="*")]
        System.Threading.Tasks.Task<ADE.Processes.SunatDelivery.SunatPRDr.getStatusResponse> getStatusAsync(ADE.Processes.SunatDelivery.SunatPRDr.getStatusRequest request);
        
        // CODEGEN: Parameter 'applicationResponse' requires additional schema information that cannot be captured using the parameter mode. The specific attribute is 'System.Xml.Serialization.XmlElementAttribute'.
        [System.ServiceModel.OperationContractAttribute(Action="urn:sendBill", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        [return: System.ServiceModel.MessageParameterAttribute(Name="applicationResponse")]
        ADE.Processes.SunatDelivery.SunatPRDr.sendBillResponse sendBill(ADE.Processes.SunatDelivery.SunatPRDr.sendBillRequest request);
        
        [System.ServiceModel.OperationContractAttribute(Action="urn:sendBill", ReplyAction="*")]
        System.Threading.Tasks.Task<ADE.Processes.SunatDelivery.SunatPRDr.sendBillResponse> sendBillAsync(ADE.Processes.SunatDelivery.SunatPRDr.sendBillRequest request);
        
        // CODEGEN: Parameter 'ticket' requires additional schema information that cannot be captured using the parameter mode. The specific attribute is 'System.Xml.Serialization.XmlElementAttribute'.
        [System.ServiceModel.OperationContractAttribute(Action="urn:sendPack", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        [return: System.ServiceModel.MessageParameterAttribute(Name="ticket")]
        ADE.Processes.SunatDelivery.SunatPRDr.sendPackResponse sendPack(ADE.Processes.SunatDelivery.SunatPRDr.sendPackRequest request);
        
        [System.ServiceModel.OperationContractAttribute(Action="urn:sendPack", ReplyAction="*")]
        System.Threading.Tasks.Task<ADE.Processes.SunatDelivery.SunatPRDr.sendPackResponse> sendPackAsync(ADE.Processes.SunatDelivery.SunatPRDr.sendPackRequest request);
        
        // CODEGEN: Parameter 'ticket' requires additional schema information that cannot be captured using the parameter mode. The specific attribute is 'System.Xml.Serialization.XmlElementAttribute'.
        [System.ServiceModel.OperationContractAttribute(Action="urn:sendSummary", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        [return: System.ServiceModel.MessageParameterAttribute(Name="ticket")]
        ADE.Processes.SunatDelivery.SunatPRDr.sendSummaryResponse sendSummary(ADE.Processes.SunatDelivery.SunatPRDr.sendSummaryRequest request);
        
        [System.ServiceModel.OperationContractAttribute(Action="urn:sendSummary", ReplyAction="*")]
        System.Threading.Tasks.Task<ADE.Processes.SunatDelivery.SunatPRDr.sendSummaryResponse> sendSummaryAsync(ADE.Processes.SunatDelivery.SunatPRDr.sendSummaryRequest request);
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.6.1064.2")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://service.sunat.gob.pe")]
    public partial class statusResponse : object, System.ComponentModel.INotifyPropertyChanged {
        
        private byte[] contentField;
        
        private string statusCodeField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, DataType="base64Binary", Order=0)]
        public byte[] content {
            get {
                return this.contentField;
            }
            set {
                this.contentField = value;
                this.RaisePropertyChanged("content");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=1)]
        public string statusCode {
            get {
                return this.statusCodeField;
            }
            set {
                this.statusCodeField = value;
                this.RaisePropertyChanged("statusCode");
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(WrapperName="getStatus", WrapperNamespace="http://service.sunat.gob.pe", IsWrapped=true)]
    public partial class getStatusRequest {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://service.sunat.gob.pe", Order=0)]
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string ticket;
        
        public getStatusRequest() {
        }
        
        public getStatusRequest(string ticket) {
            this.ticket = ticket;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(WrapperName="getStatusResponse", WrapperNamespace="http://service.sunat.gob.pe", IsWrapped=true)]
    public partial class getStatusResponse {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://service.sunat.gob.pe", Order=0)]
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public ADE.Processes.SunatDelivery.SunatPRDr.statusResponse status;
        
        public getStatusResponse() {
        }
        
        public getStatusResponse(ADE.Processes.SunatDelivery.SunatPRDr.statusResponse status) {
            this.status = status;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(WrapperName="sendBill", WrapperNamespace="http://service.sunat.gob.pe", IsWrapped=true)]
    public partial class sendBillRequest {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://service.sunat.gob.pe", Order=0)]
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string fileName;
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://service.sunat.gob.pe", Order=1)]
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, DataType="base64Binary")]
        public byte[] contentFile;
        
        public sendBillRequest() {
        }
        
        public sendBillRequest(string fileName, byte[] contentFile) {
            this.fileName = fileName;
            this.contentFile = contentFile;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(WrapperName="sendBillResponse", WrapperNamespace="http://service.sunat.gob.pe", IsWrapped=true)]
    public partial class sendBillResponse {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://service.sunat.gob.pe", Order=0)]
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, DataType="base64Binary")]
        public byte[] applicationResponse;
        
        public sendBillResponse() {
        }
        
        public sendBillResponse(byte[] applicationResponse) {
            this.applicationResponse = applicationResponse;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(WrapperName="sendPack", WrapperNamespace="http://service.sunat.gob.pe", IsWrapped=true)]
    public partial class sendPackRequest {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://service.sunat.gob.pe", Order=0)]
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string fileName;
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://service.sunat.gob.pe", Order=1)]
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, DataType="base64Binary")]
        public byte[] contentFile;
        
        public sendPackRequest() {
        }
        
        public sendPackRequest(string fileName, byte[] contentFile) {
            this.fileName = fileName;
            this.contentFile = contentFile;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(WrapperName="sendPackResponse", WrapperNamespace="http://service.sunat.gob.pe", IsWrapped=true)]
    public partial class sendPackResponse {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://service.sunat.gob.pe", Order=0)]
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string ticket;
        
        public sendPackResponse() {
        }
        
        public sendPackResponse(string ticket) {
            this.ticket = ticket;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(WrapperName="sendSummary", WrapperNamespace="http://service.sunat.gob.pe", IsWrapped=true)]
    public partial class sendSummaryRequest {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://service.sunat.gob.pe", Order=0)]
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string fileName;
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://service.sunat.gob.pe", Order=1)]
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, DataType="base64Binary")]
        public byte[] contentFile;
        
        public sendSummaryRequest() {
        }
        
        public sendSummaryRequest(string fileName, byte[] contentFile) {
            this.fileName = fileName;
            this.contentFile = contentFile;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(WrapperName="sendSummaryResponse", WrapperNamespace="http://service.sunat.gob.pe", IsWrapped=true)]
    public partial class sendSummaryResponse {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://service.sunat.gob.pe", Order=0)]
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string ticket;
        
        public sendSummaryResponse() {
        }
        
        public sendSummaryResponse(string ticket) {
            this.ticket = ticket;
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface billServiceChannel : ADE.Processes.SunatDelivery.SunatPRDr.billService, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class billServiceClient : System.ServiceModel.ClientBase<ADE.Processes.SunatDelivery.SunatPRDr.billService>, ADE.Processes.SunatDelivery.SunatPRDr.billService {
        
        public billServiceClient() {
        }
        
        public billServiceClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public billServiceClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public billServiceClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public billServiceClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        ADE.Processes.SunatDelivery.SunatPRDr.getStatusResponse ADE.Processes.SunatDelivery.SunatPRDr.billService.getStatus(ADE.Processes.SunatDelivery.SunatPRDr.getStatusRequest request) {
            return base.Channel.getStatus(request);
        }
        
        public ADE.Processes.SunatDelivery.SunatPRDr.statusResponse getStatus(string ticket) {
            ADE.Processes.SunatDelivery.SunatPRDr.getStatusRequest inValue = new ADE.Processes.SunatDelivery.SunatPRDr.getStatusRequest();
            inValue.ticket = ticket;
            ADE.Processes.SunatDelivery.SunatPRDr.getStatusResponse retVal = ((ADE.Processes.SunatDelivery.SunatPRDr.billService)(this)).getStatus(inValue);
            return retVal.status;
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        System.Threading.Tasks.Task<ADE.Processes.SunatDelivery.SunatPRDr.getStatusResponse> ADE.Processes.SunatDelivery.SunatPRDr.billService.getStatusAsync(ADE.Processes.SunatDelivery.SunatPRDr.getStatusRequest request) {
            return base.Channel.getStatusAsync(request);
        }
        
        public System.Threading.Tasks.Task<ADE.Processes.SunatDelivery.SunatPRDr.getStatusResponse> getStatusAsync(string ticket) {
            ADE.Processes.SunatDelivery.SunatPRDr.getStatusRequest inValue = new ADE.Processes.SunatDelivery.SunatPRDr.getStatusRequest();
            inValue.ticket = ticket;
            return ((ADE.Processes.SunatDelivery.SunatPRDr.billService)(this)).getStatusAsync(inValue);
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        ADE.Processes.SunatDelivery.SunatPRDr.sendBillResponse ADE.Processes.SunatDelivery.SunatPRDr.billService.sendBill(ADE.Processes.SunatDelivery.SunatPRDr.sendBillRequest request) {
            return base.Channel.sendBill(request);
        }
        
        public byte[] sendBill(string fileName, byte[] contentFile) {
            ADE.Processes.SunatDelivery.SunatPRDr.sendBillRequest inValue = new ADE.Processes.SunatDelivery.SunatPRDr.sendBillRequest();
            inValue.fileName = fileName;
            inValue.contentFile = contentFile;
            ADE.Processes.SunatDelivery.SunatPRDr.sendBillResponse retVal = ((ADE.Processes.SunatDelivery.SunatPRDr.billService)(this)).sendBill(inValue);
            return retVal.applicationResponse;
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        System.Threading.Tasks.Task<ADE.Processes.SunatDelivery.SunatPRDr.sendBillResponse> ADE.Processes.SunatDelivery.SunatPRDr.billService.sendBillAsync(ADE.Processes.SunatDelivery.SunatPRDr.sendBillRequest request) {
            return base.Channel.sendBillAsync(request);
        }
        
        public System.Threading.Tasks.Task<ADE.Processes.SunatDelivery.SunatPRDr.sendBillResponse> sendBillAsync(string fileName, byte[] contentFile) {
            ADE.Processes.SunatDelivery.SunatPRDr.sendBillRequest inValue = new ADE.Processes.SunatDelivery.SunatPRDr.sendBillRequest();
            inValue.fileName = fileName;
            inValue.contentFile = contentFile;
            return ((ADE.Processes.SunatDelivery.SunatPRDr.billService)(this)).sendBillAsync(inValue);
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        ADE.Processes.SunatDelivery.SunatPRDr.sendPackResponse ADE.Processes.SunatDelivery.SunatPRDr.billService.sendPack(ADE.Processes.SunatDelivery.SunatPRDr.sendPackRequest request) {
            return base.Channel.sendPack(request);
        }
        
        public string sendPack(string fileName, byte[] contentFile) {
            ADE.Processes.SunatDelivery.SunatPRDr.sendPackRequest inValue = new ADE.Processes.SunatDelivery.SunatPRDr.sendPackRequest();
            inValue.fileName = fileName;
            inValue.contentFile = contentFile;
            ADE.Processes.SunatDelivery.SunatPRDr.sendPackResponse retVal = ((ADE.Processes.SunatDelivery.SunatPRDr.billService)(this)).sendPack(inValue);
            return retVal.ticket;
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        System.Threading.Tasks.Task<ADE.Processes.SunatDelivery.SunatPRDr.sendPackResponse> ADE.Processes.SunatDelivery.SunatPRDr.billService.sendPackAsync(ADE.Processes.SunatDelivery.SunatPRDr.sendPackRequest request) {
            return base.Channel.sendPackAsync(request);
        }
        
        public System.Threading.Tasks.Task<ADE.Processes.SunatDelivery.SunatPRDr.sendPackResponse> sendPackAsync(string fileName, byte[] contentFile) {
            ADE.Processes.SunatDelivery.SunatPRDr.sendPackRequest inValue = new ADE.Processes.SunatDelivery.SunatPRDr.sendPackRequest();
            inValue.fileName = fileName;
            inValue.contentFile = contentFile;
            return ((ADE.Processes.SunatDelivery.SunatPRDr.billService)(this)).sendPackAsync(inValue);
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        ADE.Processes.SunatDelivery.SunatPRDr.sendSummaryResponse ADE.Processes.SunatDelivery.SunatPRDr.billService.sendSummary(ADE.Processes.SunatDelivery.SunatPRDr.sendSummaryRequest request) {
            return base.Channel.sendSummary(request);
        }
        
        public string sendSummary(string fileName, byte[] contentFile) {
            ADE.Processes.SunatDelivery.SunatPRDr.sendSummaryRequest inValue = new ADE.Processes.SunatDelivery.SunatPRDr.sendSummaryRequest();
            inValue.fileName = fileName;
            inValue.contentFile = contentFile;
            ADE.Processes.SunatDelivery.SunatPRDr.sendSummaryResponse retVal = ((ADE.Processes.SunatDelivery.SunatPRDr.billService)(this)).sendSummary(inValue);
            return retVal.ticket;
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        System.Threading.Tasks.Task<ADE.Processes.SunatDelivery.SunatPRDr.sendSummaryResponse> ADE.Processes.SunatDelivery.SunatPRDr.billService.sendSummaryAsync(ADE.Processes.SunatDelivery.SunatPRDr.sendSummaryRequest request) {
            return base.Channel.sendSummaryAsync(request);
        }
        
        public System.Threading.Tasks.Task<ADE.Processes.SunatDelivery.SunatPRDr.sendSummaryResponse> sendSummaryAsync(string fileName, byte[] contentFile) {
            ADE.Processes.SunatDelivery.SunatPRDr.sendSummaryRequest inValue = new ADE.Processes.SunatDelivery.SunatPRDr.sendSummaryRequest();
            inValue.fileName = fileName;
            inValue.contentFile = contentFile;
            return ((ADE.Processes.SunatDelivery.SunatPRDr.billService)(this)).sendSummaryAsync(inValue);
        }
    }
}
