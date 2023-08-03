import java.util.ArrayList;
import java.util.List;

public class Cart {
	private List<CartItem> Items;

	public List<CartItem> getItems() {
		return Items;
	}

	public void setItems(List<CartItem> items) {
		Items = items;
	}
	
	public void addItem(CartItem item) {
		if (Items==null) {
			Items = new ArrayList<CartItem>();
		}
		item.setRowNumber(Items.size()+1);
		Items.add(item);
	}
	
	public String toString() {
		return JsonUtil.gson.toJson(this);
	}
}
