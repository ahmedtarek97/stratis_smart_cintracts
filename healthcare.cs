using Stratis.SmartContracts;
using Stratis.SmartContracts.Standards;

/// <summary>
/// The Class of Patient smart contract
///  Contains all transactions made by the Patient in the network
/// /// </summary>

public class PatientContract : SmartContract{

/// <summary>
/// The constructor of the class
/// Intializing the admin to the sender of the transaction
/// </summary>
/// <param name="smartContractState"></param>

public PatientContract(ISmartContractState smartContractState): base(smartContractState)
{
    this.Admin = Message.Sender;
}



public Address Admin
 {
        get => PersistentState.GetAddress(nameof(Admin));
        private set => PersistentState.SetAddress(nameof(Admin), value);
 }

/// <summary>
/// Gets the patient from the data stored in the contract by sending his address.
/// </summary>
/// <param name="address"> the address of the patient to return.</param>
/// <returns>the object of the patient with the given address.</returns>
/// <remarks>
/// the return in the log is used to test the output.
/// as the api used to test in swagger does not allow returning a structure.
/// </remarks>
public Patient GetPatient(Address address)
{
        var patient =  PersistentState.GetStruct<Patient>($"Patient:{address}");
         
        Log(new TransferLog { From = patient.Id, firstname = patient.firstName, dateOfBirth = patient.dateOfBirth });
        return patient;
}

/// <summary>
/// Store a patient to the smart cintract.
/// </summary>
/// <param name="address"> The address of the patient</param>
/// <param name="value">the object of the patient</param>
private void SetPatient(Address address, Patient value)
{
        PersistentState.SetStruct($"Patient:{address}", value);
}

/// <summary>
/// Gets the medical record from the data stored in the contract by sending the patient's address.
/// </summary>
/// <param name="index" >the index of the needed record</param>
/// <param name="address" >the address of the patient</param>
/// <returns>the Medical record corresponding to the patient </returns>
public MedicalRecord GetMedicalRecord(Address address,ulong index)
{
        return PersistentState.GetStruct<MedicalRecord>($"MedicalRecord:{address}:{index}");
}

/// <summary>
/// Store a medical record to the smart cintract.
/// </summary>
/// <param name="address">address of the patient</param>
/// <param name="value">the medical record object</param>
/// <param name="index">the index of the needed medical record </param>
/// /// <remarks>
/// we used the index instead of having an array of records as stratis only supports static arrays.
/// So we instead saved the records with an index in the smart contract for better preformance.
/// </remarks>
private void SetMedicalRecord(Address address, MedicalRecord value ,ulong index )
{
        PersistentState.SetStruct($"MedicalRecord:{address}:{index}", value);
}

/// <summary>
/// a structure to what is send toh the log
/// </summary>
public struct TransferLog
    {
        [Index]
        public Address From;

        
        public string firstname;

        public string dateOfBirth;
    }

/// <summary>
/// The patient structure which contains
/// his name, home address , email ,date og birth , the hospital that he is accrptrd in
/// </summary>
public struct Patient
{
    public Address Id;
    public string firstName;
    public string lastName;
    public string adress;
    public string email;
    public string dateOfBirth;
    public Address hospital;
   
}

/// <summary>
/// Create a patient node in the network
/// </summary>
/// <param name="firstName">first name of the patient</param>
/// <param name="lastName">last name of the patient</param>
/// <param name="adress">home address</param>
/// <param name="email"></param>
/// <param name="dateOfBirth"></param>
/// <returns>the created patient</returns>
public Patient createPatient(
                            string firstName,
                            string lastName,
                            string adress,
                            string email,
                            string dateOfBirth
                            )
{

    var patient = new Patient();
    patient.Id = Message.Sender ;
    patient.firstName =firstName;
    patient.lastName =lastName;
    patient.adress =adress;
    patient.email=email;
    patient.dateOfBirth=dateOfBirth;
    SetPatient(patient.Id, patient);
    Log(new TransferLog { From = patient.Id, firstname = patient.firstName, dateOfBirth = patient.dateOfBirth });
    return patient;

}

/// <summary>
/// structure of the patient's medical record
/// </summary>

public struct MedicalRecord
{
    
    public string recordName;
    public string objectName;
    public string objectDescription;
    public string fileHash;
    public string objectType;
    public string objectCategory;
    public Address ownerId;
   
}

/// <summary>
/// Create a medical record for a patient
/// </summary>
/// <param name="recordName"></param>
/// <param name="objectName"></param>
/// <param name="objectDescription"></param>
/// <param name="fileHash"></param>
/// <param name="objectType"></param>
/// <returns></returns>
public MedicalRecord createMedicalRecord(
                                             string recordName,
                                             string objectName,
                                             string objectDescription,
                                             string fileHash,
                                             string objectType
                                        )
{

    var record = new MedicalRecord();
    record.recordName=recordName;
    record.objectName=objectName;
    record.objectDescription=objectDescription;
    record.fileHash = fileHash;
    record.objectType = objectType;
    record.ownerId=Message.Sender;

     ulong recordCount=0;
    SetMedicalRecord(Message.Sender,record,recordCount);
    recordCount++;

    return record;


}

/// <summary>
/// A transaction that a patient invoke to requst going to a hospital
/// </summary>
/// <param name="hospitalId"> the hospital that the patient needs request to go to</param>
public void grantAccessRequest(Address hospitalId)
{

    var patient = GetPatient(Message.Sender);
    patient.hospital = hospitalId;
   
}





}
