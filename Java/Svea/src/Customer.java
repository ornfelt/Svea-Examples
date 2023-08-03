public class Customer {

	private String 	NationalId;
	private String	CountryCode;
	private boolean	IsCompany;
	private Boolean	IsMale;
	private String	DateOfBirth;
	private Integer	Id;
	
	public String getNationalId() {
		return NationalId;
	}
	public void setNationalId(String nationalId) {
		NationalId = nationalId;
	}
	public String getCountryCode() {
		return CountryCode;
	}
	public void setCountryCode(String countryCode) {
		CountryCode = countryCode;
	}
	public boolean isIsCompany() {
		return IsCompany;
	}
	public void setIsCompany(boolean isCompany) {
		IsCompany = isCompany;
	}
	
	public Boolean getIsMale() {
		return IsMale;
	}
	public void setIsMale(Boolean isMale) {
		IsMale = isMale;
	}
	public String getDateOfBirth() {
		return DateOfBirth;
	}
	public void setDateOfBirth(String dateOfBirth) {
		DateOfBirth = dateOfBirth;
	}
	public Integer getId() {
		return Id;
	}
	public void setId(Integer id) {
		Id = id;
	}
}