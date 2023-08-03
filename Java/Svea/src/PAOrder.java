import java.util.List;
import com.google.gson.annotations.SerializedName;

public class PAOrder {
	@SerializedName("Id")
	private Long 	id;
	@SerializedName("Currency")
	private String	currency;
	@SerializedName("MerchantOrderId")
	private String	merchantOrderId;
	@SerializedName("OrderStatus")
	private String	orderStatus;
	@SerializedName("EmailAddress")
	private String	emailAddress;
	@SerializedName("PhoneNumber")
	private String	phoneNumber;
	@SerializedName("CustomerReference")
	private String	customerReference;
	@SerializedName("PaymentType")
	private String	paymentType;
	@SerializedName("CreationDate")
	private String	creatationDate;
	@SerializedName("NationalId")
	private String	nationalId;
	@SerializedName("IsCompany")
	private boolean	isCompany;
	@SerializedName("CancelledAmount")
	private Long	cancelledAmount;
	@SerializedName("OrderAmount")
	private	Long	orderAmount;
	
	@SerializedName("BillingAddress")
	private Address billingAddress;
	
	@SerializedName("ShippingAddress")
	private Address	shippingAddress;
	
	@SerializedName("Deliveries")
	private List<Delivery> deliveries;
	
	@SerializedName("OrderRows")
	private List<OrderRow> orderRows;

	@SerializedName("Actions")
	private List<String>	actions;
	
	@SerializedName("SveaWillBuy")
	private String	sveaWillBuy;
	
	public Long getId() {
		return id;
	}
	public void setId(Long id) {
		this.id = id;
	}
	
	public String getCurrency() {
		return currency;
	}
	public void setCurrency(String currency) {
		this.currency = currency;
	}
	
	public String getMerchantOrderId() {
		return merchantOrderId;
	}
	public void setMerchantOrderId(String merchantOrderId) {
		this.merchantOrderId = merchantOrderId;
	}
	
	@SerializedName("OrderStatus")
	public String getOrderStatus() {
		return orderStatus;
	}
	public void setOrderStatus(String orderStatus) {
		this.orderStatus = orderStatus;
	}
	public String getEmailAddress() {
		return emailAddress;
	}
	public void setEmailAddress(String emailAddress) {
		this.emailAddress = emailAddress;
	}
	public String getPhoneNumber() {
		return phoneNumber;
	}
	public void setPhoneNumber(String phoneNumber) {
		this.phoneNumber = phoneNumber;
	}
	public String getCustomerReference() {
		return customerReference;
	}
	public void setCustomerReference(String customerReference) {
		this.customerReference = customerReference;
	}
	public String getPaymentType() {
		return paymentType;
	}
	public void setPaymentType(String paymentType) {
		this.paymentType = paymentType;
	}
	public String getCreatationDate() {
		return creatationDate;
	}
	public void setCreatationDate(String creatationDate) {
		this.creatationDate = creatationDate;
	}
	public String getNationalId() {
		return nationalId;
	}
	public void setNationalId(String nationalId) {
		this.nationalId = nationalId;
	}
	public boolean isCompany() {
		return isCompany;
	}
	public void setCompany(boolean isCompany) {
		this.isCompany = isCompany;
	}
	public Long getCancelledAmount() {
		return cancelledAmount;
	}
	public void setCancelledAmount(Long cancelledAmount) {
		this.cancelledAmount = cancelledAmount;
	}
	public Long getOrderAmount() {
		return orderAmount;
	}
	public void setOrderAmount(Long orderAmount) {
		this.orderAmount = orderAmount;
	}
	public List<OrderRow> getOrderRows() {
		return orderRows;
	}
	public void setOrderRows(List<OrderRow> orderRows) {
		this.orderRows = orderRows;
	}
	public Address getBillingAddress() {
		return billingAddress;
	}
	public void setBillingAddress(Address billingAddress) {
		this.billingAddress = billingAddress;
	}
	public Address getShippingAddress() {
		return shippingAddress;
	}
	public void setShippingAddress(Address shippingAddress) {
		this.shippingAddress = shippingAddress;
	}
	public List<Delivery> getDeliveries() {
		return deliveries;
	}
	public void setDeliveries(List<Delivery> deliveries) {
		this.deliveries = deliveries;
	}
	public List<String> getActions() {
		return actions;
	}
	public void setActions(List<String> actions) {
		this.actions = actions;
	}
	public String getSveaWillBuy() {
		return sveaWillBuy;
	}
	public void setSveaWillBuy(String sveaWillBuy) {
		this.sveaWillBuy = sveaWillBuy;
	}
}