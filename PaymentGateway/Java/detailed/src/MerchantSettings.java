public class MerchantSettings {

	private String	TermsUri;
	private String	CheckoutUri;
	private String	ConfirmationUri;
	private String	PushUri;
	private String	CheckoutValidationCallBackUri;
	
	public String getTermsUri() {
		return TermsUri;
	}
	public void setTermsUri(String termsUri) {
		TermsUri = termsUri;
	}
	public String getCheckoutUri() {
		return CheckoutUri;
	}
	public void setCheckoutUri(String checkoutUri) {
		CheckoutUri = checkoutUri;
	}
	public String getConfirmationUri() {
		return ConfirmationUri;
	}
	public void setConfirmationUri(String confirmationUri) {
		ConfirmationUri = confirmationUri;
	}
	public String getPushUri() {
		return PushUri;
	}
	public void setPushUri(String pushUri) {
		PushUri = pushUri;
	}
	public String getCheckoutValidationCallBackUri() {
		return CheckoutValidationCallBackUri;
	}
	public void setCheckoutValidationCallBackUri(
			String checkoutValidationCallBackUri) {
		CheckoutValidationCallBackUri = checkoutValidationCallBackUri;
	}
	
	public String toString() {
		return JsonUtil.gson.toJson(this);
	}
}
