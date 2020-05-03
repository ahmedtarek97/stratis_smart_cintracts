using Stratis.SmartContracts;

public class DonationContract : SmartContract{

    
DonationContract(ISmartContractState smartContractState): base(smartContractState)
{
    this.Doner = Message.Sender;
}

 public Address Doner
 {
        get => PersistentState.GetAddress(nameof(Doner));
        private set => PersistentState.SetAddress(nameof(Doner), value);
 }

public enum DonationState : uint
{
        pending = 0,   // if the milestone status was not approved
        rejected = 1,  // if the project time has ended
        approved = 2,  // donation is done
 }


 public struct Donation
{
    public string donationId;
    public string eventId;
    public Address userId;
    public int donationAmount;

}


public Donation createDonations(string donationId,string eventId,int donationAmount)
{
     var donation  = new Donation();

       donation.donationId = donationId;
       donation.eventId = eventId;
       donation.userId = Doner;
       donation.donationAmount = donationAmount;

       return donation;

}

       
}