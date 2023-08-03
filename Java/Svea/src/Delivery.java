import java.util.List;
import com.google.gson.annotations.SerializedName;

public class Delivery {

	@SerializedName("Id")
	private Long	id;
	
	@SerializedName("CreationDate")
	private String	creationDate;
	
	@SerializedName("InvoiceId")
	private String	invoiceId;
	
	@SerializedName("DeliveryAmount")
	private Long	deliveryAmount;
	
	@SerializedName("CreditedAmount")
	private Long	creditedAmount;
	
	@SerializedName("Credits")
	private List<Credit> credits;
	
	@SerializedName("OrderRows")
	private List<OrderRow> orderRows;
	
	@SerializedName("Actions")
	private List<String>	actions;

	public Long getId() {
		return id;
	}

	public void setId(Long id) {
		this.id = id;
	}

	public String getCreationDate() {
		return creationDate;
	}

	public void setCreationDate(String creationDate) {
		this.creationDate = creationDate;
	}
	
	public String getInvoiceId() {
		return invoiceId;
	}

	public void setInvoiceId(String invoiceId) {
		this.invoiceId = invoiceId;
	}

	public Long getDeliveryAmount() {
		return deliveryAmount;
	}

	public void setDeliveryAmount(Long deliveryAmount) {
		this.deliveryAmount = deliveryAmount;
	}

	public Long getCreditedAmount() {
		return creditedAmount;
	}

	public void setCreditedAmount(Long creditedAmount) {
		this.creditedAmount = creditedAmount;
	}
	
	public List<Credit> getCredits() {
		return credits;
	}

	public void setCredits(List<Credit> credits) {
		this.credits = credits;
	}

	public List<OrderRow> getOrderRows() {
		return orderRows;
	}

	public void setOrderRows(List<OrderRow> orderRows) {
		this.orderRows = orderRows;
	}

	public List<String> getActions() {
		return actions;
	}

	public void setActions(List<String> actions) {
		this.actions = actions;
	}
}