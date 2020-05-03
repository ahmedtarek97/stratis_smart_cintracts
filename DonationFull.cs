using Stratis.SmartContracts;
[Deploy]

/// <summary>
/// The registration smart contract
/// it contains all the transactions for the user to register
/// </summary>
public class RegistrationContract : SmartContract
{
    /// <summary>
    /// The constructor of the class
    /// Intializing the admin to the sender of the transaction
    /// </summary>
    public RegistrationContract(ISmartContractState smartContractState)
   : base(smartContractState)
    {
        this.Admin = Message.Sender;
    }
​
    /// <summary>
    /// Getting the total number of registerd users
    /// </summary>
    public int Index
    {
        get => this.Users.Length;
        
    }
    /// <summary>
    /// Return the user with a spicific index
    /// </summary>
    /// <param name="index">the index of the user</param>
    /// <returns>the address of the user</returns>
    public Address User(int index)
    {
        return this.Users[index];
       
    }
    public Address Admin
    {
        get => PersistentState.GetAddress(nameof(Admin));
        private set => PersistentState.SetAddress(nameof(Admin), value);
    }
    /// <summary>
    /// set and get the array of registerd users
    /// </summary>
    public Address[] Users
    {
        get => PersistentState.GetArray<Address>((nameof(Users)));
        private set => PersistentState.SetArray(nameof(Users), value);
    }
    /// <summary>
    /// Create a new user node in the network
    /// </summary>
    /// <returns>the address of the created users</returns>
    public Address createAccount()
    {
        var createResult = Create<UserWalletContract>(0, new object[] { Message.Sender , this.Admin});
        Assert(createResult.Success);
        Address[] memoryUsers = this.GetArrayCopy(this.Users);
​
        memoryUsers[this.Index] = createResult.NewContractAddress;
        Users = memoryUsers;
        return createResult.NewContractAddress;
    }
    // we should avoid this solution as looping is gas consumer
    /// <summary>
    /// helper function to copy the content of an array to another array.
    /// </summary>
    /// <param name="users">the array to be copied</param>
    /// <returns>the array resulted in the coping</returns>
    private Address[] GetArrayCopy(Address[] users)
    {
        var TempArray = new Address[this.Index + 1];
        for (int i = 0; i < this.Index; i++)
        {
            TempArray[i] = users[i];
​
        }
        return TempArray;
    }
    
}
​
/// <summary>
/// The smartcontract of the wallet of the user
/// </summary>
public class UserWalletContract : SmartContract
{
    /// <summary>
    /// The constructor of the smart contract
    /// </summary>
    /// <param name="smartContractState"></param>
    /// <param name="Owner">the user that made the transaction</param>
    /// <param name="Admin">The admin of the network</param>
    public UserWalletContract(ISmartContractState smartContractState, Address Owner, Address Admin)
: base(smartContractState)
    {
        this.Admin = Admin;
        this.Owner = Owner;
    }
    /// <summary>
    /// The status of the smart contract
    /// </summary>
    enum StatusType : uint
    {
        Initialized = 0,
        Rejected = 1,
        Approved = 2, Banned = 3,
        Submited = 4
    }
    public Address Owner
    {
        get => PersistentState.GetAddress(nameof(Owner));
        private set => PersistentState.SetAddress(nameof(Owner), value);
    }
    public Address Admin
    {
        get => PersistentState.GetAddress(nameof(Admin));
        private set => PersistentState.SetAddress(nameof(Admin), value);
    }
    public string Name
    {
        get => PersistentState.GetString(nameof(Name));
        private set => PersistentState.SetString(nameof(Name), value);
    }
    public string Licence
    {
        get => PersistentState.GetString(nameof(Licence));
        private set => PersistentState.SetString(nameof(Licence), value);
    }
    public string AuditReport
    {
        get => PersistentState.GetString(nameof(AuditReport));
        private set => PersistentState.SetString(nameof(AuditReport), value);
    }
    public string Passport
    {
        get => PersistentState.GetString(nameof(Passport));
        private set => PersistentState.SetString(nameof(Passport), value);
    }
​
    public string BankAccount
    {
        get => PersistentState.GetString(nameof(BankAccount));
        private set => PersistentState.SetString(nameof(BankAccount), value);
    }
    public uint State
    {
        get => PersistentState.GetUInt32(nameof(State));
        private set => PersistentState.SetUInt32(nameof(State), value);
    }
    public Address[] Campaigns
    {
        get => PersistentState.GetArray<Address>(nameof(Campaigns));
        private set => PersistentState.SetArray(nameof(Campaigns), value);
    }
    public Address CryptoAddress
    {
        get => PersistentState.GetAddress(nameof(CryptoAddress));
        private set => PersistentState.SetAddress(nameof(CryptoAddress), value);
    }
        public int Index
    {
        get => this.Campaigns.Length;
    }
    /// <summary>
    /// Creating the request to have an account on the network
    /// </summary>
    /// <param name="Licence"></param>
    /// <param name="AuditReport"></param>
    /// <param name="Passport"></param>
    /// <param name="Name"></param>
    /// <param name="BankAccount"></param>
    /// <param name="CryptoAddress"></param>
    /// <returns>it always return true</returns>
    public bool RequestToPublish(string Licence,
          string AuditReport,
          string Passport,
          string Name,
​
          string BankAccount,
          Address CryptoAddress)
    {
        Assert(this.Message.Sender == this.Owner);
        Assert(this.State != (uint)StatusType.Banned);
        this.Licence = Licence;
        this.AuditReport = AuditReport;
        this.Passport = Passport;
        this.Name = Name;
​
        this.BankAccount = BankAccount;
        this.CryptoAddress = CryptoAddress;
        this.State = (uint)StatusType.Submited;
        return true;
    }
    /// <summary>
    /// The admin manges the requests from the users.
    /// </summary>
    /// <param name="status">the status will be given to the request</param>
    /// <returns>it always return true if succesful</returns>
    public bool AdminManageRequestToPublish(uint status)
    {
        Assert(this.Message.Sender == this.Admin);
        Assert(status == (uint)StatusType.Approved || status == (uint)StatusType.Rejected || status == (uint)StatusType.Banned);
​
​
            this.State = status;
​
        return true;
    }
    /// <summary>
    /// Transaction to create a donation campaign
    /// </summary>
    /// <param name="Cap"></param>
    /// <param name="Name">name of the campaign</param>
    /// <param name="EndDate"> ending date of the campaign</param>
    /// <returns>Adress for the created event</returns>
    public Address IssueCampaign(ulong Cap, string Name, ulong EndDate)
    {
        Assert(this.State==(uint)StatusType.Approved);
         Assert(this.Message.Sender == this.Owner);
         /*Address owner, Address Admin, Address WalletContract, ulong Cap, string Name, ulong EndDate */
        var createResult = Create<CampaignContract>(0, new object[] {  this.Owner, this.Admin,this.Address, Cap,  Name,  EndDate });
        Assert(createResult.Success);
        Address[] TempArray = this.GetArrayCopy(this.Campaigns);
​       
        TempArray[this.Index] = createResult.NewContractAddress;
        Campaigns = TempArray;
        return createResult.NewContractAddress;
    }
        private Address[] GetArrayCopy(Address[] arr)
    {
        var TempArray = new Address[this.Index + 1];
        for (int i = 0; i < this.Index; i++)
        {
            TempArray[i] = arr[i];
​
        }
        return TempArray;
    }
}
​
​/// <summary>
/// The campaign smart contract
/// it contains all the transactions for the Campaign
/// </summary>
public class CampaignContract : SmartContract
{
    public enum StatusType : uint
    {
        Issued = 0,
        Submited = 1,
        Rejected = 2,
        Opened = 3,
        Finished = 4
    }
    // this for dao stages
    /// <summary>
    /// The three milestones for any campaign 
    /// </summary>
    public enum Stages : uint
    {
        Stage1 = 0,
        Stage2 = 1,
        Stage3 = 2
    }
    /// <summary>
    /// Intializing the campaign contract
    /// </summary>
    /// <param name="smartContractState"></param>
    /// <param name="owner"></param>
    /// <param name="Admin"></param>
    /// <param name="WalletContract"></param>
    /// <param name="Cap"></param>
    /// <param name="Name"></param>
    /// <param name="EndDate"></param>
    public CampaignContract(ISmartContractState smartContractState, Address owner, Address Admin, Address WalletContract, ulong Cap, string Name, ulong EndDate)
    : base(smartContractState)
    {
        this.Owner = owner;
        this.Admin = Admin;
        this.Name = Name;
        this.WalletContract = WalletContract;
        this.EndDate = EndDate;
        this.Cap = Cap;
        this.State = (uint)StatusType.Issued;
    }
​
​
    public Address Owner
    {
        get => PersistentState.GetAddress(nameof(Owner));
        private set => PersistentState.SetAddress(nameof(Owner), value);
    }
    public Address Admin
    {
        get => PersistentState.GetAddress(nameof(Admin));
        private set => PersistentState.SetAddress(nameof(Admin), value);
    }
    // later on we can use the parent class to add extra feature
    public Address WalletContract
    {
        get => PersistentState.GetAddress(nameof(WalletContract));
        private set => PersistentState.SetAddress(nameof(WalletContract), value);
    }
​
    public string Name
    {
        get => PersistentState.GetString(nameof(Name));
        private set => PersistentState.SetString(nameof(Name), value);
    }
    public string Licence
    {
        get => PersistentState.GetString(nameof(Licence));
        private set => PersistentState.SetString(nameof(Licence), value);
    }
    public string AuditReport
    {
        get => PersistentState.GetString(nameof(AuditReport));
        private set => PersistentState.SetString(nameof(AuditReport), value);
    }
    public string Passport
    {
        get => PersistentState.GetString(nameof(Passport));
        private set => PersistentState.SetString(nameof(Passport), value);
    }
    public string MangerName
    {
        get => PersistentState.GetString(nameof(MangerName));
        private set => PersistentState.SetString(nameof(MangerName), value);
    }
    public string BankAccount
    {
        get => PersistentState.GetString(nameof(BankAccount));
        private set => PersistentState.SetString(nameof(BankAccount), value);
    }
    public Address CryptoAddress
    {
        get => PersistentState.GetAddress(nameof(CryptoAddress));
        private set => PersistentState.SetAddress(nameof(CryptoAddress), value);
    }
​
    public ulong StartDate
    {
        get => PersistentState.GetUInt64(nameof(StartDate));
        private set => PersistentState.SetUInt64(nameof(StartDate), value);
    }
    public ulong EndDate
    {
        get => PersistentState.GetUInt64(nameof(EndDate));
        private set => PersistentState.SetUInt64(nameof(EndDate), value);
    }
    public ulong TotalSupply
    {
        get => PersistentState.GetUInt64(nameof(this.TotalSupply));
        private set => PersistentState.SetUInt64(nameof(this.TotalSupply), value);
    }
    public ulong Cap
    {
        get => PersistentState.GetUInt64(nameof(this.Cap));
        private set => PersistentState.SetUInt64(nameof(this.Cap), value);
    }
    public uint State
    {
        get => PersistentState.GetUInt32(nameof(State));
        private set => PersistentState.SetUInt32(nameof(State), value);
    }
    /// <summary>
    /// geeting the current balance of the campaign
    /// </summary>
    /// <param name="address">the address of the campaign</param>
    /// <returns></returns>
    public ulong GetBalance(Address address)
    {
        return PersistentState.GetUInt64($"Balance:{address}");
    }
​
    private void SetBalance(Address address, ulong value)
    {
        PersistentState.SetUInt64($"Balance:{address}", value);
    }
    /// <summary>
    /// Tranfering balance to another node int the network
    /// </summary>
    /// <param name="to">the address of the node</param>
    /// <param name="amount">the amount of the transformation wanted</param>
    /// <returns></returns>
    private bool TransferTo(Address to, ulong amount)
    {
        if (amount == 0)
        {
​
​
            return false;
        }
​
        //  use checked and unchecked to prevent overflow & or to overflow
        SetBalance(to, checked(GetBalance(to) + amount));
​
        return true;
    }

    /// <summary>
    /// Creating the request to have a campaign 
    /// </summary>
    /// <param name="Licence"></param>
    /// <param name="AuditReport"></param>
    /// <param name="Passport"></param>
    /// <param name="Name"></param>
    /// <param name="BankAccount"></param>
    /// <param name="CryptoAddress"></param>
    /// <returns>it always return true</returns>
    public bool RequestToPublish(string Licence,
          string AuditReport,
          string Passport,
          string MangerName,
​
          string BankAccount,
          Address CryptoAddress)
    {
        Assert(this.Message.Sender == this.Owner);
        this.Licence = Licence;
        this.AuditReport = AuditReport;
        this.Passport = Passport;
        this.MangerName = MangerName;
​        this.BankAccount = BankAccount;
        this.CryptoAddress = CryptoAddress;
        this.State = (uint)StatusType.Submited;
        return true;
    }
    /// <summary>
    /// The admin manges the requests from the campaigns.
    /// </summary>
    /// <param name="status">the status will be given to the request</param>
    /// <returns>it always return true if succesful</returns>
    public bool AdminManageRequestToPublish(bool status)
    {
        Assert(this.Message.Sender == this.Admin);
        if (status)
        {
            this.State = (uint)StatusType.Opened;
            this.StartDate = this.Block.Number;
        }
        else
        {
            this.State = (uint)StatusType.Rejected;
        }
        return true;
    }
    /// <summary>
    /// Create a donation
    /// </summary>
    public void Donate()
    {
        Assert(this.Block.Number < this.EndDate);
        Assert(this.TotalSupply < this.Cap);
        Assert(this.State == (uint)StatusType.Opened);
        Assert(this.Message.Value > 0);
        Assert(this.TransferTo(this.Message.Sender, this.Message.Value));
        this.TotalSupply += this.Message.Value;
​
    }
​    
    /// <summary>
    /// Withdrwaing money from the campaign balance
    /// </summary>
    public bool Withdraw()
    {
        Assert(this.State == (uint)StatusType.Finished);
        // we will add extra functionality here by implementing dao 
        ITransferResult transferResult = Transfer(this.Owner, this.Balance);
​
        return transferResult.Success;
    }
​
}
​
​
public class FactoryContract : SmartContract
{
    public FactoryContract(ISmartContractState smartContractState, Address owner)
    : base(smartContractState)
    {
        this.Admin = owner;
    }
    public Address Admin
    {
        get => PersistentState.GetAddress(nameof(Admin));
        private set => PersistentState.SetAddress(nameof(Admin), value);
    }
    public Address RegistrationFactory
    {
        get => PersistentState.GetAddress(nameof(RegistrationFactory));
        private set => PersistentState.SetAddress(nameof(RegistrationFactory), value);
    }
}