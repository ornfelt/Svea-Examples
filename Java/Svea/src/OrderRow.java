import com.google.gson.annotations.SerializedName;

public class OrderRow {

	@SerializedName("OrderRowId")
	private Long	orderRowId;
	@SerializedName("ArticleNumber")
	private String	articleNumber;
	@SerializedName("Name")
	private String	name;
	@SerializedName("Quantity")
	private Long	quantity;
	@SerializedName("UnitPrice")
	private Long	unitPrice;
	@SerializedName("DiscountPercent")
	private Long	discountPercent;
	@SerializedName("VatPercent")
	private Long	vatPercent;
	@SerializedName("Unit")
	private String	unit;
	@SerializedName("IsCancelled")
	private boolean	isCancelled;
	
	public Long getOrderRowId() {
		return orderRowId;
	}
	public void setOrderRowId(Long orderRowId) {
		this.orderRowId = orderRowId;
	}
	public String getArticleNumber() {
		return articleNumber;
	}
	public void setArticleNumber(String articleNumber) {
		this.articleNumber = articleNumber;
	}
	public String getName() {
		return name;
	}
	public void setName(String name) {
		this.name = name;
	}
	public Long getQuantity() {
		return quantity;
	}
	public void setQuantity(Long quantity) {
		this.quantity = quantity;
	}
	public Long getUnitPrice() {
		return unitPrice;
	}
	public void setUnitPrice(Long unitPrice) {
		this.unitPrice = unitPrice;
	}
	public Long getDiscountPercent() {
		return discountPercent;
	}
	public void setDiscountPercent(Long discountPercent) {
		this.discountPercent = discountPercent;
	}
	public Long getVatPercent() {
		return vatPercent;
	}
	public void setVatPercent(Long vatPercent) {
		this.vatPercent = vatPercent;
	}
	public String getUnit() {
		return unit;
	}
	public void setUnit(String unit) {
		this.unit = unit;
	}
	public boolean isCancelled() {
		return isCancelled;
	}
	public void setCancelled(boolean isCancelled) {
		this.isCancelled = isCancelled;
	}
}