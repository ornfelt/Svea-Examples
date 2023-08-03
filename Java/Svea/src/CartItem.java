public class CartItem {

	private String 	ArticleNumber;
	private String 	Name;
	private Integer	Quantity;
	private Integer	UnitPrice;
	private Integer	DiscountPercent;
	private Integer	VatPercent;
	private String	Unit;
	private String	TemporaryReference;
	private Integer	RowNumber;
	
	public String getArticleNumber() {
		return ArticleNumber;
	}
	public void setArticleNumber(String articleNumber) {
		ArticleNumber = articleNumber;
	}
	public String getName() {
		return Name;
	}
	public void setName(String name) {
		Name = name;
	}
	
	public Integer getQuantity() {
		return Quantity;
	}
	public void setQuantity(Integer quantity) {
		Quantity = quantity;
	}
	
	public void setRealQuantity(double realQty) {
		Quantity = Integer.valueOf((int)Math.round(realQty * 100));
	}

	public double getRealQuantity() {
		return (Quantity/100);
	}
	
	public Integer getUnitPrice() {
		return UnitPrice;
	}
	public void setUnitPrice(Integer unitPrice) {
		UnitPrice = unitPrice;
	}
	
	public void setRealUnitPrice(double realUnitPrice) {
		UnitPrice = Integer.valueOf((int)Math.round(realUnitPrice * 100));
	}
	
	public Integer getDiscountPercent() {
		return DiscountPercent;
	}
	public void setDiscountPercent(Integer discountPercent) {
		DiscountPercent = discountPercent;
	}
	public Integer getVatPercent() {
		return VatPercent;
	}
	public void setVatPercent(Integer vatPercent) {
		VatPercent = vatPercent;
	}
	public String getUnit() {
		return Unit;
	}
	public void setUnit(String unit) {
		Unit = unit;
	}
	public String getTemporaryReference() {
		return TemporaryReference;
	}
	public void setTemporaryReference(String temporaryReference) {
		TemporaryReference = temporaryReference;
	}
	public Integer getRowNumber() {
		return RowNumber;
	}
	public void setRowNumber(Integer rowNumber) {
		RowNumber = rowNumber;
	}
	
	public String toString() {
		return JsonUtil.gson.toJson(this);
	}
}
