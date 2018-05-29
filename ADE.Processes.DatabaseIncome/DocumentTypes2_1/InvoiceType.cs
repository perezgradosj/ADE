using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Xml.Serialization;

namespace ADE.Processes.DatabaseIncome.DocumentTypes2_1
{
    [XmlRoot(ElementName = "CanonicalizationMethod", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
    public class CanonicalizationMethod
    {
        [XmlAttribute(AttributeName = "Algorithm")]
        public string Algorithm { get; set; }
    }

    [XmlRoot(ElementName = "SignatureMethod", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
    public class SignatureMethod
    {
        [XmlAttribute(AttributeName = "Algorithm")]
        public string Algorithm { get; set; }
    }

    [XmlRoot(ElementName = "Transform", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
    public class Transform
    {
        [XmlAttribute(AttributeName = "Algorithm")]
        public string Algorithm { get; set; }
    }

    [XmlRoot(ElementName = "Transforms", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
    public class Transforms
    {
        [XmlElement(ElementName = "Transform", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
        public Transform Transform { get; set; }
    }

    [XmlRoot(ElementName = "DigestMethod", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
    public class DigestMethod
    {
        [XmlAttribute(AttributeName = "Algorithm")]
        public string Algorithm { get; set; }
    }

    [XmlRoot(ElementName = "Reference", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
    public class Reference
    {
        [XmlElement(ElementName = "Transforms", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
        public Transforms Transforms { get; set; }
        [XmlElement(ElementName = "DigestMethod", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
        public DigestMethod DigestMethod { get; set; }
        [XmlElement(ElementName = "DigestValue", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
        public string DigestValue { get; set; }
        [XmlAttribute(AttributeName = "URI")]
        public string URI { get; set; }
    }

    [XmlRoot(ElementName = "SignedInfo", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
    public class SignedInfo
    {
        [XmlElement(ElementName = "CanonicalizationMethod", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
        public CanonicalizationMethod CanonicalizationMethod { get; set; }
        [XmlElement(ElementName = "SignatureMethod", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
        public SignatureMethod SignatureMethod { get; set; }
        [XmlElement(ElementName = "Reference", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
        public Reference Reference { get; set; }
    }

    [XmlRoot(ElementName = "X509Data", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
    public class X509Data
    {
        [XmlElement(ElementName = "X509SubjectName", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
        public string X509SubjectName { get; set; }
        [XmlElement(ElementName = "X509Certificate", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
        public string X509Certificate { get; set; }
    }

    [XmlRoot(ElementName = "KeyInfo", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
    public class KeyInfo
    {
        [XmlElement(ElementName = "X509Data", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
        public X509Data X509Data { get; set; }
    }

    [XmlRoot(ElementName = "Signature", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
    public class Signature
    {
        [XmlElement(ElementName = "SignedInfo", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
        public SignedInfo SignedInfo { get; set; }
        [XmlElement(ElementName = "SignatureValue", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
        public string SignatureValue { get; set; }
        [XmlElement(ElementName = "KeyInfo", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
        public KeyInfo KeyInfo { get; set; }
        [XmlAttribute(AttributeName = "Id")]
        public string Id { get; set; }
    }

    [XmlRoot(ElementName = "ExtensionContent", Namespace = "urn:oasis:names:specification:ubl:schema:xsd:CommonExtensionComponents-2")]
    public class ExtensionContent
    {
        [XmlElement(ElementName = "Signature", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
        public Signature Signature { get; set; }
    }

    [XmlRoot(ElementName = "UBLExtension", Namespace = "urn:oasis:names:specification:ubl:schema:xsd:CommonExtensionComponents-2")]
    public class UBLExtension
    {
        [XmlElement(ElementName = "ExtensionContent", Namespace = "urn:oasis:names:specification:ubl:schema:xsd:CommonExtensionComponents-2")]
        public ExtensionContent ExtensionContent { get; set; }
    }

    [XmlRoot(ElementName = "UBLExtensions", Namespace = "urn:oasis:names:specification:ubl:schema:xsd:CommonExtensionComponents-2")]
    public class UBLExtensions
    {
        [XmlElement(ElementName = "UBLExtension", Namespace = "urn:oasis:names:specification:ubl:schema:xsd:CommonExtensionComponents-2")]
        public UBLExtension UBLExtension { get; set; }
    }

    [XmlRoot(ElementName = "ProfileID", Namespace = "urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2")]
    public class ProfileID
    {
        [XmlAttribute(AttributeName = "schemeName")]
        public string SchemeName { get; set; }
        [XmlAttribute(AttributeName = "schemeAgencyName")]
        public string SchemeAgencyName { get; set; }
        [XmlAttribute(AttributeName = "schemeURI")]
        public string SchemeURI { get; set; }
        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "InvoiceTypeCode", Namespace = "urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2")]
    public class InvoiceTypeCode
    {
        [XmlAttribute(AttributeName = "listAgencyName")]
        public string ListAgencyName { get; set; }
        [XmlAttribute(AttributeName = "listName")]
        public string ListName { get; set; }
        [XmlAttribute(AttributeName = "listURI")]
        public string ListURI { get; set; }
        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "Note", Namespace = "urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2")]
    public class Note
    {
        [XmlAttribute(AttributeName = "languageLocaleID")]
        public string LanguageLocaleID { get; set; }
        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "DocumentCurrencyCode", Namespace = "urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2")]
    public class DocumentCurrencyCode
    {
        [XmlAttribute(AttributeName = "listID")]
        public string ListID { get; set; }
        [XmlAttribute(AttributeName = "listName")]
        public string ListName { get; set; }
        [XmlAttribute(AttributeName = "listAgencyName")]
        public string ListAgencyName { get; set; }
        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "OrderReference", Namespace = "urn:oasis:names:specification:ubl:schema:xsd:CommonAggregateComponents-2")]
    public class OrderReference
    {
        [XmlElement(ElementName = "ID", Namespace = "urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2")]
        public string ID { get; set; }
    }

    [XmlRoot(ElementName = "DocumentTypeCode", Namespace = "urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2")]
    public class DocumentTypeCode
    {
        [XmlAttribute(AttributeName = "listAgencyName")]
        public string ListAgencyName { get; set; }
        [XmlAttribute(AttributeName = "listName")]
        public string ListName { get; set; }
        [XmlAttribute(AttributeName = "listURI")]
        public string ListURI { get; set; }
        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "DespatchDocumentReference", Namespace = "urn:oasis:names:specification:ubl:schema:xsd:CommonAggregateComponents-2")]
    public class DespatchDocumentReference
    {
        [XmlElement(ElementName = "ID", Namespace = "urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2")]
        public string ID { get; set; }
        [XmlElement(ElementName = "DocumentTypeCode", Namespace = "urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2")]
        public DocumentTypeCode DocumentTypeCode { get; set; }
    }

    [XmlRoot(ElementName = "PartyIdentification", Namespace = "urn:oasis:names:specification:ubl:schema:xsd:CommonAggregateComponents-2")]
    public class PartyIdentification
    {
        [XmlElement(ElementName = "ID", Namespace = "urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2")]
        public string ID { get; set; }
    }

    [XmlRoot(ElementName = "PartyName", Namespace = "urn:oasis:names:specification:ubl:schema:xsd:CommonAggregateComponents-2")]
    public class PartyName
    {
        [XmlElement(ElementName = "Name", Namespace = "urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2")]
        public string Name { get; set; }
    }

    [XmlRoot(ElementName = "SignatoryParty", Namespace = "urn:oasis:names:specification:ubl:schema:xsd:CommonAggregateComponents-2")]
    public class SignatoryParty
    {
        [XmlElement(ElementName = "PartyIdentification", Namespace = "urn:oasis:names:specification:ubl:schema:xsd:CommonAggregateComponents-2")]
        public PartyIdentification PartyIdentification { get; set; }
        [XmlElement(ElementName = "PartyName", Namespace = "urn:oasis:names:specification:ubl:schema:xsd:CommonAggregateComponents-2")]
        public PartyName PartyName { get; set; }
    }

    [XmlRoot(ElementName = "ExternalReference", Namespace = "urn:oasis:names:specification:ubl:schema:xsd:CommonAggregateComponents-2")]
    public class ExternalReference
    {
        [XmlElement(ElementName = "URI", Namespace = "urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2")]
        public string URI { get; set; }
    }

    [XmlRoot(ElementName = "DigitalSignatureAttachment", Namespace = "urn:oasis:names:specification:ubl:schema:xsd:CommonAggregateComponents-2")]
    public class DigitalSignatureAttachment
    {
        [XmlElement(ElementName = "ExternalReference", Namespace = "urn:oasis:names:specification:ubl:schema:xsd:CommonAggregateComponents-2")]
        public ExternalReference ExternalReference { get; set; }
    }

    [XmlRoot(ElementName = "Signature", Namespace = "urn:oasis:names:specification:ubl:schema:xsd:CommonAggregateComponents-2")]
    public class Signature2
    {
        [XmlElement(ElementName = "ID", Namespace = "urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2")]
        public string ID { get; set; }
        [XmlElement(ElementName = "SignatoryParty", Namespace = "urn:oasis:names:specification:ubl:schema:xsd:CommonAggregateComponents-2")]
        public SignatoryParty SignatoryParty { get; set; }
        [XmlElement(ElementName = "DigitalSignatureAttachment", Namespace = "urn:oasis:names:specification:ubl:schema:xsd:CommonAggregateComponents-2")]
        public DigitalSignatureAttachment DigitalSignatureAttachment { get; set; }
    }

    [XmlRoot(ElementName = "CompanyID", Namespace = "urn:oasis:names:specification:ubl:schema:xsd:Invoice-2")]
    public class CompanyID
    {
        [XmlAttribute(AttributeName = "schemeID")]
        public string SchemeID { get; set; }
        [XmlAttribute(AttributeName = "schemeName")]
        public string SchemeName { get; set; }
        [XmlAttribute(AttributeName = "schemeAgencyName")]
        public string SchemeAgencyName { get; set; }
        [XmlAttribute(AttributeName = "schemeURI")]
        public string SchemeURI { get; set; }
        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "RegistrationAddress", Namespace = "urn:oasis:names:specification:ubl:schema:xsd:CommonAggregateComponents-2")]
    public class RegistrationAddress
    {
        [XmlElement(ElementName = "AddressTypeCode", Namespace = "urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2")]
        public string AddressTypeCode { get; set; }
    }

    [XmlRoot(ElementName = "TaxScheme", Namespace = "urn:oasis:names:specification:ubl:schema:xsd:CommonAggregateComponents-2")]
    public class TaxScheme
    {
        [XmlElement(ElementName = "ID", Namespace = "urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2")]
        public ID ID { get; set; }
        [XmlElement(ElementName = "Name", Namespace = "urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2")]
        public string Name { get; set; }
        [XmlElement(ElementName = "TaxTypeCode", Namespace = "urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2")]
        public string TaxTypeCode { get; set; }
    }

    [XmlRoot(ElementName = "PartyTaxScheme", Namespace = "urn:oasis:names:specification:ubl:schema:xsd:CommonAggregateComponents-2")]
    public class PartyTaxScheme
    {
        [XmlElement(ElementName = "RegistrationName", Namespace = "urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2")]
        public string RegistrationName { get; set; }
        [XmlElement(ElementName = "CompanyID", Namespace = "urn:oasis:names:specification:ubl:schema:xsd:Invoice-2")]
        public CompanyID CompanyID { get; set; }
        [XmlElement(ElementName = "RegistrationAddress", Namespace = "urn:oasis:names:specification:ubl:schema:xsd:CommonAggregateComponents-2")]
        public RegistrationAddress RegistrationAddress { get; set; }
        [XmlElement(ElementName = "TaxScheme", Namespace = "urn:oasis:names:specification:ubl:schema:xsd:CommonAggregateComponents-2")]
        public TaxScheme TaxScheme { get; set; }
    }

    [XmlRoot(ElementName = "Party", Namespace = "urn:oasis:names:specification:ubl:schema:xsd:CommonAggregateComponents-2")]
    public class Party
    {
        [XmlElement(ElementName = "PartyName", Namespace = "urn:oasis:names:specification:ubl:schema:xsd:CommonAggregateComponents-2")]
        public PartyName PartyName { get; set; }
        [XmlElement(ElementName = "PartyTaxScheme", Namespace = "urn:oasis:names:specification:ubl:schema:xsd:CommonAggregateComponents-2")]
        public PartyTaxScheme PartyTaxScheme { get; set; }
    }

    [XmlRoot(ElementName = "AccountingSupplierParty", Namespace = "urn:oasis:names:specification:ubl:schema:xsd:CommonAggregateComponents-2")]
    public class AccountingSupplierParty
    {
        [XmlElement(ElementName = "Party", Namespace = "urn:oasis:names:specification:ubl:schema:xsd:CommonAggregateComponents-2")]
        public Party Party { get; set; }
    }

    [XmlRoot(ElementName = "AccountingCustomerParty", Namespace = "urn:oasis:names:specification:ubl:schema:xsd:CommonAggregateComponents-2")]
    public class AccountingCustomerParty
    {
        [XmlElement(ElementName = "Party", Namespace = "urn:oasis:names:specification:ubl:schema:xsd:CommonAggregateComponents-2")]
        public Party Party { get; set; }
    }

    [XmlRoot(ElementName = "TaxAmount", Namespace = "urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2")]
    public class TaxAmount
    {
        [XmlAttribute(AttributeName = "currencyID")]
        public string CurrencyID { get; set; }
        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "TaxableAmount", Namespace = "urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2")]
    public class TaxableAmount
    {
        [XmlAttribute(AttributeName = "currencyID")]
        public string CurrencyID { get; set; }
        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "ID", Namespace = "urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2")]
    public class ID
    {
        [XmlAttribute(AttributeName = "schemeID")]
        public string SchemeID { get; set; }
        [XmlAttribute(AttributeName = "schemeName")]
        public string SchemeName { get; set; }
        [XmlAttribute(AttributeName = "schemeAgencyName")]
        public string SchemeAgencyName { get; set; }
        [XmlText]
        public string Text { get; set; }
        [XmlAttribute(AttributeName = "schemeAgencyID")]
        public string SchemeAgencyID { get; set; }
    }

    [XmlRoot(ElementName = "TaxCategory", Namespace = "urn:oasis:names:specification:ubl:schema:xsd:CommonAggregateComponents-2")]
    public class TaxCategory
    {
        [XmlElement(ElementName = "ID", Namespace = "urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2")]
        public ID ID { get; set; }
        [XmlElement(ElementName = "TaxScheme", Namespace = "urn:oasis:names:specification:ubl:schema:xsd:CommonAggregateComponents-2")]
        public TaxScheme TaxScheme { get; set; }
        [XmlElement(ElementName = "Percent", Namespace = "urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2")]
        public string Percent { get; set; }
        [XmlElement(ElementName = "TaxExemptionReasonCode", Namespace = "urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2")]
        public TaxExemptionReasonCode TaxExemptionReasonCode { get; set; }
        [XmlElement(ElementName = "TierRange", Namespace = "urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2")]
        public string TierRange { get; set; }
    }

    [XmlRoot(ElementName = "TaxSubtotal", Namespace = "urn:oasis:names:specification:ubl:schema:xsd:CommonAggregateComponents-2")]
    public class TaxSubtotal
    {
        [XmlElement(ElementName = "TaxableAmount", Namespace = "urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2")]
        public TaxableAmount TaxableAmount { get; set; }
        [XmlElement(ElementName = "TaxAmount", Namespace = "urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2")]
        public TaxAmount TaxAmount { get; set; }
        [XmlElement(ElementName = "TaxCategory", Namespace = "urn:oasis:names:specification:ubl:schema:xsd:CommonAggregateComponents-2")]
        public TaxCategory TaxCategory { get; set; }
    }

    [XmlRoot(ElementName = "TaxTotal", Namespace = "urn:oasis:names:specification:ubl:schema:xsd:CommonAggregateComponents-2")]
    public class TaxTotal
    {
        [XmlElement(ElementName = "TaxAmount", Namespace = "urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2")]
        public TaxAmount TaxAmount { get; set; }
        [XmlElement(ElementName = "TaxSubtotal", Namespace = "urn:oasis:names:specification:ubl:schema:xsd:CommonAggregateComponents-2")]
        public List<TaxSubtotal> TaxSubtotal { get; set; }
    }

    [XmlRoot(ElementName = "LineExtensionAmount", Namespace = "urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2")]
    public class LineExtensionAmount
    {
        [XmlAttribute(AttributeName = "currencyID")]
        public string CurrencyID { get; set; }
        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "TaxInclusiveAmount", Namespace = "urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2")]
    public class TaxInclusiveAmount
    {
        [XmlAttribute(AttributeName = "currencyID")]
        public string CurrencyID { get; set; }
        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "AllowanceTotalAmount", Namespace = "urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2")]
    public class AllowanceTotalAmount
    {
        [XmlAttribute(AttributeName = "currencyID")]
        public string CurrencyID { get; set; }
        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "ChargeTotalAmount", Namespace = "urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2")]
    public class ChargeTotalAmount
    {
        [XmlAttribute(AttributeName = "currencyID")]
        public string CurrencyID { get; set; }
        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "PayableAmount", Namespace = "urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2")]
    public class PayableAmount
    {
        [XmlAttribute(AttributeName = "currencyID")]
        public string CurrencyID { get; set; }
        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "LegalMonetaryTotal", Namespace = "urn:oasis:names:specification:ubl:schema:xsd:CommonAggregateComponents-2")]
    public class LegalMonetaryTotal
    {
        [XmlElement(ElementName = "LineExtensionAmount", Namespace = "urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2")]
        public LineExtensionAmount LineExtensionAmount { get; set; }
        [XmlElement(ElementName = "TaxInclusiveAmount", Namespace = "urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2")]
        public TaxInclusiveAmount TaxInclusiveAmount { get; set; }
        [XmlElement(ElementName = "AllowanceTotalAmount", Namespace = "urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2")]
        public AllowanceTotalAmount AllowanceTotalAmount { get; set; }
        [XmlElement(ElementName = "ChargeTotalAmount", Namespace = "urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2")]
        public ChargeTotalAmount ChargeTotalAmount { get; set; }
        [XmlElement(ElementName = "PayableAmount", Namespace = "urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2")]
        public PayableAmount PayableAmount { get; set; }
    }

    [XmlRoot(ElementName = "InvoicedQuantity", Namespace = "urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2")]
    public class InvoicedQuantity
    {
        [XmlAttribute(AttributeName = "unitCode")]
        public string UnitCode { get; set; }
        [XmlAttribute(AttributeName = "unitCodeListID")]
        public string UnitCodeListID { get; set; }
        [XmlAttribute(AttributeName = "unitCodeListAgencyName")]
        public string UnitCodeListAgencyName { get; set; }
        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "PriceAmount", Namespace = "urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2")]
    public class PriceAmount
    {
        [XmlAttribute(AttributeName = "currencyID")]
        public string CurrencyID { get; set; }
        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "PriceTypeCode", Namespace = "urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2")]
    public class PriceTypeCode
    {
        [XmlAttribute(AttributeName = "listName")]
        public string ListName { get; set; }
        [XmlAttribute(AttributeName = "listAgencyName")]
        public string ListAgencyName { get; set; }
        [XmlAttribute(AttributeName = "listURI")]
        public string ListURI { get; set; }
        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "AlternativeConditionPrice", Namespace = "urn:oasis:names:specification:ubl:schema:xsd:CommonAggregateComponents-2")]
    public class AlternativeConditionPrice
    {
        [XmlElement(ElementName = "PriceAmount", Namespace = "urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2")]
        public PriceAmount PriceAmount { get; set; }
        [XmlElement(ElementName = "PriceTypeCode", Namespace = "urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2")]
        public PriceTypeCode PriceTypeCode { get; set; }
    }

    [XmlRoot(ElementName = "PricingReference", Namespace = "urn:oasis:names:specification:ubl:schema:xsd:CommonAggregateComponents-2")]
    public class PricingReference
    {
        [XmlElement(ElementName = "AlternativeConditionPrice", Namespace = "urn:oasis:names:specification:ubl:schema:xsd:CommonAggregateComponents-2")]
        public AlternativeConditionPrice AlternativeConditionPrice { get; set; }
    }

    [XmlRoot(ElementName = "Amount", Namespace = "urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2")]
    public class Amount
    {
        [XmlAttribute(AttributeName = "currencyID")]
        public string CurrencyID { get; set; }
        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "BaseAmount", Namespace = "urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2")]
    public class BaseAmount
    {
        [XmlAttribute(AttributeName = "currencyID")]
        public string CurrencyID { get; set; }
        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "AllowanceCharge", Namespace = "urn:oasis:names:specification:ubl:schema:xsd:CommonAggregateComponents-2")]
    public class AllowanceCharge
    {
        [XmlElement(ElementName = "ChargeIndicator", Namespace = "urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2")]
        public string ChargeIndicator { get; set; }
        [XmlElement(ElementName = "AllowanceChargeReasonCode", Namespace = "urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2")]
        public string AllowanceChargeReasonCode { get; set; }
        [XmlElement(ElementName = "MultiplierFactorNumeric", Namespace = "urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2")]
        public string MultiplierFactorNumeric { get; set; }
        [XmlElement(ElementName = "Amount", Namespace = "urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2")]
        public Amount Amount { get; set; }
        [XmlElement(ElementName = "BaseAmount", Namespace = "urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2")]
        public BaseAmount BaseAmount { get; set; }
    }

    [XmlRoot(ElementName = "TaxExemptionReasonCode", Namespace = "urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2")]
    public class TaxExemptionReasonCode
    {
        [XmlAttribute(AttributeName = "listAgencyName")]
        public string ListAgencyName { get; set; }
        [XmlAttribute(AttributeName = "listName")]
        public string ListName { get; set; }
        [XmlAttribute(AttributeName = "listURI")]
        public string ListURI { get; set; }
        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "SellersItemIdentification", Namespace = "urn:oasis:names:specification:ubl:schema:xsd:CommonAggregateComponents-2")]
    public class SellersItemIdentification
    {
        [XmlElement(ElementName = "ID", Namespace = "urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2")]
        public string ID { get; set; }
    }

    [XmlRoot(ElementName = "ItemClassificationCode", Namespace = "urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2")]
    public class ItemClassificationCode
    {
        [XmlAttribute(AttributeName = "listID")]
        public string ListID { get; set; }
        [XmlAttribute(AttributeName = "listAgencyName")]
        public string ListAgencyName { get; set; }
        [XmlAttribute(AttributeName = "listName")]
        public string ListName { get; set; }
        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "CommodityClassification", Namespace = "urn:oasis:names:specification:ubl:schema:xsd:CommonAggregateComponents-2")]
    public class CommodityClassification
    {
        [XmlElement(ElementName = "ItemClassificationCode", Namespace = "urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2")]
        public ItemClassificationCode ItemClassificationCode { get; set; }
    }

    [XmlRoot(ElementName = "Item", Namespace = "urn:oasis:names:specification:ubl:schema:xsd:CommonAggregateComponents-2")]
    public class Item
    {
        [XmlElement(ElementName = "Description", Namespace = "urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2")]
        public string Description { get; set; }
        [XmlElement(ElementName = "SellersItemIdentification", Namespace = "urn:oasis:names:specification:ubl:schema:xsd:CommonAggregateComponents-2")]
        public SellersItemIdentification SellersItemIdentification { get; set; }
        [XmlElement(ElementName = "CommodityClassification", Namespace = "urn:oasis:names:specification:ubl:schema:xsd:CommonAggregateComponents-2")]
        public CommodityClassification CommodityClassification { get; set; }
    }

    [XmlRoot(ElementName = "Price", Namespace = "urn:oasis:names:specification:ubl:schema:xsd:CommonAggregateComponents-2")]
    public class Price
    {
        [XmlElement(ElementName = "PriceAmount", Namespace = "urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2")]
        public PriceAmount PriceAmount { get; set; }
    }

    [XmlRoot(ElementName = "InvoiceLine", Namespace = "urn:oasis:names:specification:ubl:schema:xsd:CommonAggregateComponents-2")]
    public class InvoiceLine
    {
        [XmlElement(ElementName = "ID", Namespace = "urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2")]
        public string ID { get; set; }
        [XmlElement(ElementName = "InvoicedQuantity", Namespace = "urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2")]
        public InvoicedQuantity InvoicedQuantity { get; set; }
        [XmlElement(ElementName = "LineExtensionAmount", Namespace = "urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2")]
        public LineExtensionAmount LineExtensionAmount { get; set; }
        [XmlElement(ElementName = "PricingReference", Namespace = "urn:oasis:names:specification:ubl:schema:xsd:CommonAggregateComponents-2")]
        public PricingReference PricingReference { get; set; }
        [XmlElement(ElementName = "AllowanceCharge", Namespace = "urn:oasis:names:specification:ubl:schema:xsd:CommonAggregateComponents-2")]
        public AllowanceCharge AllowanceCharge { get; set; }
        [XmlElement(ElementName = "TaxTotal", Namespace = "urn:oasis:names:specification:ubl:schema:xsd:CommonAggregateComponents-2")]
        public TaxTotal TaxTotal { get; set; }
        [XmlElement(ElementName = "Item", Namespace = "urn:oasis:names:specification:ubl:schema:xsd:CommonAggregateComponents-2")]
        public Item Item { get; set; }
        [XmlElement(ElementName = "Price", Namespace = "urn:oasis:names:specification:ubl:schema:xsd:CommonAggregateComponents-2")]
        public Price Price { get; set; }
    }

    [XmlRoot(ElementName = "Invoice", Namespace = "urn:oasis:names:specification:ubl:schema:xsd:Invoice-2")]
    public class Invoice
    {
        [XmlElement(ElementName = "UBLExtensions", Namespace = "urn:oasis:names:specification:ubl:schema:xsd:CommonExtensionComponents-2")]
        public UBLExtensions UBLExtensions { get; set; }
        [XmlElement(ElementName = "UBLVersionID", Namespace = "urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2")]
        public string UBLVersionID { get; set; }
        [XmlElement(ElementName = "CustomizationID", Namespace = "urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2")]
        public string CustomizationID { get; set; }
        [XmlElement(ElementName = "ProfileID", Namespace = "urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2")]
        public ProfileID ProfileID { get; set; }
        [XmlElement(ElementName = "ID", Namespace = "urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2")]
        public string ID { get; set; }
        [XmlElement(ElementName = "IssueDate", Namespace = "urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2")]
        public string IssueDate { get; set; }
        [XmlElement(ElementName = "IssueTime", Namespace = "urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2")]
        public string IssueTime { get; set; }
        [XmlElement(ElementName = "InvoiceTypeCode", Namespace = "urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2")]
        public InvoiceTypeCode InvoiceTypeCode { get; set; }
        [XmlElement(ElementName = "Note", Namespace = "urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2")]
        public List<Note> Note { get; set; }
        [XmlElement(ElementName = "DocumentCurrencyCode", Namespace = "urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2")]
        public DocumentCurrencyCode DocumentCurrencyCode { get; set; }
        [XmlElement(ElementName = "LineCountNumeric", Namespace = "urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2")]
        public string LineCountNumeric { get; set; }
        [XmlElement(ElementName = "OrderReference", Namespace = "urn:oasis:names:specification:ubl:schema:xsd:CommonAggregateComponents-2")]
        public OrderReference OrderReference { get; set; }
        [XmlElement(ElementName = "DespatchDocumentReference", Namespace = "urn:oasis:names:specification:ubl:schema:xsd:CommonAggregateComponents-2")]
        public DespatchDocumentReference DespatchDocumentReference { get; set; }
        [XmlElement(ElementName = "Signature", Namespace = "urn:oasis:names:specification:ubl:schema:xsd:CommonAggregateComponents-2")]
        public Signature2 Signature2 { get; set; }
        [XmlElement(ElementName = "AccountingSupplierParty", Namespace = "urn:oasis:names:specification:ubl:schema:xsd:CommonAggregateComponents-2")]
        public AccountingSupplierParty AccountingSupplierParty { get; set; }
        [XmlElement(ElementName = "AccountingCustomerParty", Namespace = "urn:oasis:names:specification:ubl:schema:xsd:CommonAggregateComponents-2")]
        public AccountingCustomerParty AccountingCustomerParty { get; set; }
        [XmlElement(ElementName = "TaxTotal", Namespace = "urn:oasis:names:specification:ubl:schema:xsd:CommonAggregateComponents-2")]
        public TaxTotal TaxTotal { get; set; }
        [XmlElement(ElementName = "LegalMonetaryTotal", Namespace = "urn:oasis:names:specification:ubl:schema:xsd:CommonAggregateComponents-2")]
        public LegalMonetaryTotal LegalMonetaryTotal { get; set; }
        [XmlElement(ElementName = "InvoiceLine", Namespace = "urn:oasis:names:specification:ubl:schema:xsd:CommonAggregateComponents-2")]
        public List<InvoiceLine> InvoiceLine { get; set; }
        [XmlAttribute(AttributeName = "xmlns")]
        public string Xmlns { get; set; }
        [XmlAttribute(AttributeName = "cac", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Cac { get; set; }
        [XmlAttribute(AttributeName = "cbc", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Cbc { get; set; }
        [XmlAttribute(AttributeName = "ccts", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Ccts { get; set; }
        [XmlAttribute(AttributeName = "ds", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Ds { get; set; }
        [XmlAttribute(AttributeName = "ext", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Ext { get; set; }
        [XmlAttribute(AttributeName = "qdt", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Qdt { get; set; }
        [XmlAttribute(AttributeName = "udt", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Udt { get; set; }
        [XmlAttribute(AttributeName = "xsi", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Xsi { get; set; }
    }

}
