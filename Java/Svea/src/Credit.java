import java.util.List;
import com.google.gson.annotations.SerializedName;

public class Credit {

	@SerializedName("Amount")
	private Long amount;
	
	@SerializedName("OrderRows")
	private List<OrderRow>	orderRows;
	
	@SerializedName("Actions")
	private List<String>	actions;

	public Long getAmount() {
		return amount;
	}

	public void setAmount(Long amount) {
		this.amount = amount;
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