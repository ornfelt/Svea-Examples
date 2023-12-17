import com.google.gson.annotations.SerializedName;

public class Order {

	@SerializedName("MerchantSettings")
	private MerchantSettings merchantSettings;
	@SerializedName("Cart")
	private Cart cart;
	
	@SerializedName("Customer")
	private Customer customer;
	
	@SerializedName("ShippingAddress")
	private Address shippingAddress;
	
	@SerializedName("BillingAddress")
	private Address billingAddress;
	
	@SerializedName("Gui")
	private Gui gui;
	
	private String	Locale;
	private String	Currency;
	private String	CountryCode;
	private String	PresetValues;
	private String	ClientOrderNumber;
	private String	OrderId;
	private String	EmailAddress;
	private String	PhoneNumber;
	private String	PaymentType;
	private String	Status;
	private String	CustomerReference;
	private String	SveaWillBuyOrder;
	
	public MerchantSettings getMerchantSettings() {
		return merchantSettings;
	}
	public void setMerchantSettings(MerchantSettings merchantSettings) {
		this.merchantSettings = merchantSettings;
	}
	public Cart getCart() {
		return cart;
	}
	public void setCart(Cart cart) {
		this.cart = cart;
	}
	
	public Customer getCustomer() {
		return customer;
	}
	public void setCustomer(Customer customer) {
		this.customer = customer;
	}
	public String getLocale() {
		return Locale;
	}
	public void setLocale(String locale) {
		Locale = locale;
	}
	public String getCountryCode() {
		return CountryCode;
	}
	public void setCountryCode(String countryCode) {
		CountryCode = countryCode;
	}
	public String getCurrency() {
		return Currency;
	}
	public void setCurrency(String currency) {
		Currency = currency;
	}
	public String getClientOrderNumber() {
		return ClientOrderNumber;
	}
	public void setClientOrderNumber(String clientOrderNumber) {
		ClientOrderNumber = clientOrderNumber;
	}
	public Address getShippingAddress() {
		return shippingAddress;
	}
	public void setShippingAddress(Address shippingAddress) {
		this.shippingAddress = shippingAddress;
	}
	public Address getBillingAddress() {
		return billingAddress;
	}
	public void setBillingAddress(Address billingAddress) {
		this.billingAddress = billingAddress;
	}
	public Gui getGui() {
		return gui;
	}
	public void setGui(Gui gui) {
		this.gui = gui;
	}
	public String getPresetValues() {
		return PresetValues;
	}
	public void setPresetValues(String presetValues) {
		PresetValues = presetValues;
	}
	public String getOrderId() {
		return OrderId;
	}
	public void setOrderId(String orderId) {
		OrderId = orderId;
	}
	public String getEmailAddress() {
		return EmailAddress;
	}
	public void setEmailAddress(String emailAddress) {
		EmailAddress = emailAddress;
	}
	public String getPhoneNumber() {
		return PhoneNumber;
	}
	public void setPhoneNumber(String phoneNumber) {
		PhoneNumber = phoneNumber;
	}
	public String getPaymentType() {
		return PaymentType;
	}
	public void setPaymentType(String paymentType) {
		PaymentType = paymentType;
	}
	public String getStatus() {
		return Status;
	}
	public void setStatus(String status) {
		Status = status;
	}
	public String getCustomerReference() {
		return CustomerReference;
	}
	public void setCustomerReference(String customerReference) {
		CustomerReference = customerReference;
	}
	public String getSveaWillBuyOrder() {
		return SveaWillBuyOrder;
	}
	public void setSveaWillBuyOrder(String sveaWillBuyOrder) {
		SveaWillBuyOrder = sveaWillBuyOrder;
	}
	
	public String toString() {
		return JsonUtil.gson.toJson(this);
	}
}
