using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.IO;

namespace Hw1_Hashing
{
    public class Program
    {

        static List<PhoneData> PhoneData1 = new List<PhoneData>(); //arraylist for phone number
        static List<PersonData> PersonData1 = new List<PersonData>(); //arraylist for person 
        static Hashtable Hash = new Hashtable();
        static string path = @"D:\PhoneBook.txt";

        static void Main(string[] args)
        {
            try
            {
                readData();
                char x;
                bool isExit = false;
                do
                {
                    Console.Write("Phonebook - Input your command (h for help):");
                    string m = Console.ReadLine();
                    x = char.ToLower(m[0]);

                    switch (x)
                    {
                        case 'i': insert(); //func insert a phone number 
                            break;
                        case 'd': delete(); //delete a phone number
                            break;
                        case 'm': modify(); //modify
                            break;
                        case 's':
                            {
                                search(); //search phone no
                                break;
                            }
                        case 'x':
                            {
                                isExit = true;
                                //write data to textfiles
                                writeData();
                                break;
                            }
                        case 'h':
                            {
                                help();
                                break;
                            }
                        default: break;
                    }

                } while (!isExit);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Program Error : " + ex.Message);
            }
        }

        static void insert()
        {
            bool m = false;
            string x = string.Empty;
            string newList = "The new list is:";
            do
            {
                Console.WriteLine("Enter a phone number to be inserted: ");
                x = Console.ReadLine();
                x = x.Trim();
                int y = x.Length;
                if (y > 10)
                    Console.WriteLine("Error - the number is too long");
                else if (y < 10)
                    Console.WriteLine("Error - the number is too short");
                else
                {
                    m = true;
                    foreach (PhoneData item in PhoneData1)
                    {

                        if (String.Compare(item.PhoneNumber, x) == 0)
                        {
                            Console.WriteLine("Error - the phone number is already in this phone book. Please insert the new one.: ");
                            m = false;
                            break;
                        }

                    }
                }
            } while (m);

            Console.WriteLine("The list is:");
            showList(null);

            //Show for Enter input new person or not
            Console.WriteLine("Enter the person id to append the phone number or /'n/' for new phone number :");
            string inputNP = Console.ReadLine();
            int no;
            //if it new person
            if ("nN".Contains(inputNP))
            {
                Console.WriteLine("Enter the name (8 Characters maximum) :");
                string inputName = Console.ReadLine();
                while (inputName.Length > 8)
                {
                    Console.WriteLine("Error name is more that 8 Characters, Please Enter new name:");
                    inputName = Console.ReadLine();
                }

                //insert new name
                PersonData p = new PersonData();
                p.PersonNo = PersonData1.Count + 1;
                p.Name = inputName;
                p.CountPhoneNo++;
                PersonData1.Add(p);

                //insert new phone no. to list
                PhoneData ph = new PhoneData();
                ph.no = p.PersonNo;
                ph.PhoneNumber = x;
                ph.phoneOrder = 1;
                PhoneData1.Add(ph);

                //add key to hashtable
                InsertHashTable(ph);
                //print new list
                Console.WriteLine(newList);
                showList(null);
            }
            else if (int.TryParse(inputNP, out no) == true)
            {
                int noIndex = PersonData1.FindIndex(i => i.PersonNo == no);
                //checking is it full 5 phone no for that person
                //if it full show error and input the replace no for replace that phone no
                if (PersonData1[noIndex].CountPhoneNo == 5)
                {
                    Console.WriteLine(PersonData1[noIndex] + " has 5 phone numbers already, enter replaced number or any key for cancel:");
                    string inputRC = Console.ReadLine();
                    int tryParseInputRC;
                    //if not cancel replace that phone no to another one
                    if (int.TryParse(inputRC, out tryParseInputRC))
                    {
                        //replace and remove key of that phone no. from hash table
                        //int indexperson = PersonData1.FindIndex(i => i.PersonNo == PersonData1[no].PersonNo);
                        PhoneData ph = new PhoneData();
                        ph.no = no;
                        int indexphone = PhoneData1.FindIndex(i => i.phoneOrder == tryParseInputRC);
                        ph.PhoneNumber = PhoneData1[indexphone].PhoneNumber;
                        ph.phoneOrder = tryParseInputRC;

                        PhoneData ph2 = new PhoneData();
                        ph2.no = ph.no;
                        ph2.PhoneNumber = x;
                        ph2.phoneOrder = ph.phoneOrder;
                        PhoneData1[indexphone] = ph2;

                        //delete hash table data
                        DeleteHashTable(ph);

                        //insert new hash table data
                        InsertHashTable(ph2);

                        //show new list
                        Console.WriteLine(newList);
                        showList(null);
                    }
                }
                else
                {
                    Console.WriteLine("Enter replaced number or /'n/' for new phone number:");
                    string inputPHID = Console.ReadLine();
                    if ("nN".Contains(inputPHID))
                    {
                        //add new phone number and position
                        PhoneData ph = new PhoneData();
                        ph.phoneOrder = PersonData1[noIndex].CountPhoneNo + 1;
                        ph.no = no;
                        ph.PhoneNumber = x;
                        PhoneData1.Add(ph);

                        //increase phone item in person
                        int indexPer = PersonData1.FindIndex(i => i.PersonNo == no);

                        //insert phone no to hash table
                        InsertHashTable(ph);
                    }
                    else
                    {
                        int phoneN;
                        bool isNumberic = int.TryParse(inputPHID, out phoneN);
                        //ask to replace phone no.
                        if (phoneN <= PersonData1[noIndex].CountPhoneNo)
                        {
                            int j = 0;
                            for (int i = 0; i < PhoneData1.Count; i++)
                            {
                                if (PhoneData1[i].phoneOrder == phoneN)
                                {
                                    j = i;
                                    break;
                                }
                            }

                            string p1 = PhoneData1[j].PhoneNumber;
                            p1 = p1.Substring(0, 3) + " " + p1.Substring(3, 3) + " " + p1.Substring(6, 4);
                            string p2 = x;
                            p2 = p2.Substring(0, 3) + " " + p2.Substring(3, 3) + " " + p2.Substring(6, 4);
                            Console.WriteLine("Do you want to replace " + PersonData1[noIndex].Name + "'s contact " + p1 + " with " + p2 + " ('/y'/ for yes, any key to cancel)? :");
                            string checkR = Console.ReadLine();
                            if ("yY".Contains(checkR))
                            {
                                //replace that phone no.
                                PhoneData ph1 = new PhoneData();
                                ph1.no = no;
                                ph1.PhoneNumber = x;
                                ph1.phoneOrder = phoneN;
                                PhoneData1[j] = ph1;

                                PhoneData ph2 = new PhoneData();
                                ph2.no = no;
                                ph2.PhoneNumber = p2.Trim();
                                ph2.phoneOrder = phoneN;

                                //remove hash table about that phone no.
                                DeleteHashTable(ph2);

                                //insert new hash table of this phone no.
                                InsertHashTable(ph1);
                            }
                        }
                        else
                        {
                            //insert phone no in next position
                            PhoneData ph = new PhoneData();
                            ph.PhoneNumber = x;
                            ph.no = no;
                            ph.phoneOrder = PersonData1[noIndex].CountPhoneNo + 1;
                            PhoneData1.Add(ph);

                            //insert to hash table
                            InsertHashTable(ph);
                        }
                    }
                    //print new list
                    Console.WriteLine(newList);
                    showList(null);
                }
            }

        }

        static void readData()
        {

            if (File.Exists(path))
            {
                string[] lines = System.IO.File.ReadAllLines(path);
                //string line = string.Empty;

                PersonData p = null;
                PhoneData ph = null;
                int i = 1;
                foreach (string line in lines)
                {
                    string[] spline = line.Split(' ');
                    //if a person
                    if (spline.Count() > 1)
                    {
                        //add old one
                        if (p != null)
                        {
                            PersonData1.Add(p);
                        }
                        //is a person
                        p = new PersonData();
                        p.PersonNo = Convert.ToInt16(spline[0]);
                        p.Name = spline[1];
                        i = 1;
                    }
                    else
                    {
                        //is a phone no
                        p.CountPhoneNo++;
                        ph = new PhoneData();
                        ph.no = p.PersonNo;
                        ph.PhoneNumber = spline[0];
                        ph.phoneOrder = i;
                        i++;
                        PhoneData1.Add(ph);
                        //insert into hash table
                        InsertHashTable(ph);
                    }
                }

            }
        }

        static void writeData()
        {
            using (System.IO.StreamWriter file =
            new System.IO.StreamWriter(path))
            {

                foreach (PersonData item in PersonData1)
                {
                    file.WriteLine(item.PersonNo.ToString() + " " + item.Name);
                    foreach (PhoneData phdata in PhoneData1)
                    {
                        if (phdata.no == item.PersonNo)
                            file.WriteLine(phdata.PhoneNumber);
                    }
                }
            }
        }

        static void InsertHashTable(PhoneData ph)
        {
            string key;
            for (int i = 0; i < 7; i++)
            {
                key = ph.PhoneNumber.Substring(i, 3);
                // value = person index
                if (Hash.ContainsKey(key))
                {
                    string x = Hash[key].ToString();
                    string[] m = x.Split(' ');
                    bool isHavInHashing = false;
                    foreach (string item in m)
                    {
                        if (Convert.ToInt16(item) == ph.no)
                        {
                            isHavInHashing = true;
                            break;
                        }
                    }
                    if (!isHavInHashing)
                    {
                        // add phone person no to hashing
                        Hash[key] = Hash[key].ToString() + " " + ph.no.ToString();
                    }

                }
                else
                {
                    Hash.Add(key, ph.no.ToString());
                }
            }
        }

        static void DeleteHashTable(PhoneData ph)
        {
            string key;
            for (int i = 0; i < 7; i++)
            {
                key = ph.PhoneNumber.Substring(i, 3);
                string x = Hash[key].ToString();
                string[] item = x.Split(' ');
                bool isHavSameHash = false;
                foreach (string m in item)
                {
                    //if the same person
                    if (ph.no == Convert.ToInt16(m))
                    {
                        foreach (PhoneData photh in PhoneData1)
                        {
                            if (photh.no == ph.no && ph.PhoneNumber != photh.PhoneNumber)
                            {
                                isHavSameHash = true;
                                break;
                            }
                        }
                    }
                }
                x = string.Empty;
                if (!isHavSameHash)
                {
                    for (int j = 0; j < item.Length; i++)
                    {
                        if (ph.no == Convert.ToInt16(item[j]))
                        {
                            item[j] = string.Empty;
                        }
                        x = x + item[j] + " ";
                    }
                }
                Hash[key] = x;
            }

        }

        static void help()
        {
            string data = "i - insert a phone number" + Environment.NewLine + "d - delete a phone number" + Environment.NewLine + "m - modify a phone number" + Environment.NewLine + "s - search" + Environment.NewLine + "x - exit program";

            Console.WriteLine(data);
        }

        static void showList(string key)
        {
            //show full list if key = null
            if (key == null)
            {
                foreach (PersonData PersonItem in PersonData1)
                {
                    Console.Write("[" + PersonItem.PersonNo.ToString() + "] ");
                    int j = 1;
                    for (int i = 0; i < PhoneData1.Count; i++)
                    {
                        if (PhoneData1[i].no == PersonItem.PersonNo)
                        {
                            string pNumber = PhoneData1[i].PhoneNumber;
                            string PhoneNumber = pNumber.Substring(0, 3) + " " + pNumber.Substring(3, 3) + " " + pNumber.Substring(7, 4) + Environment.NewLine;
                            int k = PersonData1[i].Name.Length;
                            if (j == 1)
                            {
                                Console.Write(" " + PhoneData1[i].no + " - " + PhoneNumber);
                            }
                            else
                            {
                                for (int n = 0; n < k + 4; n++)
                                {
                                    Console.Write(" ");
                                }
                                Console.Write(" " + PhoneData1[i].no + " - " + PhoneNumber);
                            }
                            j++;
                        }
                    }
                }
            }
            //otherwise cal key in hash table and get true phone no
            bool isHaveValue = false;
            string mkey = Hash[key].ToString();
            string[] splitkey = mkey.Split(' ');
            foreach (string pkey in splitkey)
            {
                int indexPerson = PersonData1.FindIndex(i => i.PersonNo == Convert.ToInt16(pkey));
                Console.Write("[" + PersonData1[indexPerson].PersonNo.ToString() + "] ");
                int j = 1;
                for (int i = 0; i < PhoneData1.Count; i++)
                {
                    if (PhoneData1[i].no == Convert.ToInt16(pkey))
                    {
                        string pNumber = PhoneData1[i].PhoneNumber;
                        string PhoneNumber = pNumber.Substring(0, 3) + " " + pNumber.Substring(3, 3) + " " + pNumber.Substring(7, 4) + Environment.NewLine;
                        int k = PersonData1[i].Name.Length;
                        if (j == 1)
                        {
                            Console.Write(" " + PhoneData1[i].no + " - " + PhoneNumber);
                        }
                        else
                        {
                            for (int n = 0; n < k + 4; n++)
                            {
                                Console.Write(" ");
                            }
                            Console.Write(" " + PhoneData1[i].no + " - " + PhoneNumber);
                        }
                        j++;
                    }
                }
                if (j > 1)
                    isHaveValue = true;
            }
            if (!isHaveValue)
            {
                Console.WriteLine("No Result");
            }

        }

        static void modify()
        {
            //modify data
            //show old list
            Console.WriteLine("the list is:");
            showList(null);

            //get replace person phone no.
            Console.WriteLine("Enter the person id to be modified or any key to cancel :");
            string no = Console.ReadLine();
            int Personid;
            if(int.TryParse(no,out Personid))
            {
                Console.WriteLine("Enter the phone number id to be modified :");
                string phoneno = Console.ReadLine();
                int phoneNum;
                bool isPhoneNum = int.TryParse(phoneno,out phoneNum);
                if (isPhoneNum)
                {

                    int indexPerson = PersonData1.FindIndex(i=> i.PersonNo == Personid);
                    if (phoneNum <= PersonData1[indexPerson].CountPhoneNo)
                    {
                        Console.WriteLine("Enter the new phone number :");
                        string phonenumber = Console.ReadLine();

                        string disph1 = phonenumber.Substring(0, 3) + " " + phonenumber.Substring(3, 3) + " " + phonenumber.Substring(6, 4);
                        //compare
                        int indexPhone = PhoneData1.FindIndex(i => i.phoneOrder == phoneNum && i.no == Personid);
                        string number = PhoneData1[indexPhone].PhoneNumber;
                        string disph2 = number.Substring(0, 3) + " " + number.Substring(3, 3) + " " + number.Substring(6, 4);
                        Console.WriteLine("Do you want to modify " + PersonData1[indexPerson].Name + "'s contact from " + disph2 + " to " + disph1 + " ('/y'/ for yes, any key to cancel)? :");
                        string checkY = Console.ReadLine();

                        //if 'y' replace phone number
                        if ("yY".Contains(checkY))
                        {
                            //old phone no
                            PhoneData ph = new PhoneData();
                            ph.no = Personid;
                            ph.PhoneNumber = number;
                            ph.phoneOrder = phoneNum;

                            DeleteHashTable(ph);

                            PhoneData1[indexPhone].PhoneNumber = phonenumber;
                            //new phone no
                            ph.PhoneNumber = phonenumber;

                            InsertHashTable(ph);

                            //show new list
                            Console.WriteLine("The new list is :");
                            showList(null);

                        }
                        //else do nothing
                    }
                    else
                    {
                        Console.WriteLine("Error there is no phone id for modify, please input new command");
                    }
                }
                else
                {
                    Console.WriteLine("Error it is not a phone number, please input new command");

                }

            }
            
        }

        static void delete()
        {
            //delete data
            Console.WriteLine("The list is :");
            showList(null);
            Console.WriteLine("Enter the person id to be deleted or any key to cancel:");
            string pid = Console.ReadLine();
            int personid;
            if(int.TryParse(pid, out personid))
            {
                if (personid < PersonData1.Count)
                {
                    int indexPerson = PersonData1.FindIndex(i => i.PersonNo == personid);
                    if (PersonData1[indexPerson].CountPhoneNo == 1)
                    {
                        //delete person data and phone data
                        Console.WriteLine("Do you want to delete "+PersonData1[indexPerson].Name+"'s contact as well ('/y'/ for yes, any key to cancel)? :");
                        string checkY = Console.ReadLine();
                        if ("yY".Contains(checkY))
                        {
                            int indexPhone = PhoneData1.FindIndex(i=> i.no == personid);
                            PhoneData ph = PhoneData1[indexPhone];
                            DeleteHashTable(ph);
                            PersonData1.RemoveAt(indexPerson);
                            PhoneData1.RemoveAt(indexPhone);
                            
                            //new list
                            Console.WriteLine("The new list is:");
                            showList(null);
                        }

                    }
                    else
                    {
                        //delete phone data only
                        Console.WriteLine("Enter the phone number id to be deleted :");
                        string phid = Console.ReadLine();
                        int phoneid;

                        //checking is phone id is more than count
                        if (int.TryParse(phid, out phoneid))
                        {
                            if (phoneid > PersonData1[indexPerson].CountPhoneNo)
                            {
                                Console.WriteLine("Error, there is no this phone id. Please, input new command.");
                            }
                            else
                            {
                                Console.WriteLine("Confirm ('/y'/ for yes, any key to cancel)? :");
                                string confirm = Console.ReadLine();
                                if ("Yy".Contains(confirm))
                                {
                                    int indexPhone = PhoneData1.FindIndex(i => i.phoneOrder == phoneid && i.no == personid);
                                    PhoneData ph = PhoneData1[indexPhone];
                                    PhoneData1.RemoveAt(indexPhone);
                                    DeleteHashTable(ph);
                                }
                            }
                        }
                    }
                }
                else
                {
                    Console.WriteLine("Error, there is no this person id. Please, input new command.");
                }
            }
        }

        static void search()
        {
            //search 3 key ---on hash table
            Console.WriteLine("Enter 3 digits to search:");
            string x = string.Empty;
            bool isNumber = false;
            do
            {

                x = Console.ReadLine();
                if (x.Length != 3)
                {
                    Console.WriteLine("Error, there is no 3 digits.");
                }
                int number;
                isNumber = int.TryParse(x,out number);
                if (!isNumber)
                {
                    Console.WriteLine("Error, the input is not all number.");
                }

            } while (x.Length != 3 || !isNumber);
            showList(x);
        }

    }

    public class PhoneData
    {
        public int no { get; set; }
        public String PhoneNumber { get; set; }
        public int phoneOrder { get; set; }
    }
    public class PersonData
    {
        public int CountPhoneNo { get; set; }
        public String Name { get; set; }
        public int PersonNo { get; set; }

        public PersonData()
        {
            CountPhoneNo = 0;
        }
    }
    
}
