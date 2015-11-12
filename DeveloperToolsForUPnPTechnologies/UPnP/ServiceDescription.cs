using System;

namespace Intel.UPNP
{
	/// <summary>
	/// A convenience class that encapsulated a Service Description
	/// </summary>
	public class ServiceDescription
	{
		public int Major;
		public int Minor;

		public String ServiceID;
		public String ServiceType;

		public String SCPDURL;
		public String ControlURL;
		public String EventURL;

		public bool IsStandardService;

		/// <summary>
		/// Initializes a new instance
		/// </summary>
		/// <param name="version">Version Number</param>
		/// <param name="Service_ID">USN, if blank a new GUID will be created for you</param>
		/// <param name="Service_Type">The Type</param>
		/// <param name="IsStandard">True if standard, false if domain specific</param>
		public ServiceDescription(double version, String Service_ID, String Service_Type, bool IsStandard)
		{
			IsStandardService = IsStandard;
			if(Service_ID=="")
			{
				ServiceID = Guid.NewGuid().ToString();
			}
			else
			{
				ServiceID = Service_ID;
			}
			ServiceType = Service_Type;

			SCPDURL = "{" + Service_ID + "}scpd.xml";
			ControlURL = "{" + Service_ID + "}control";
			EventURL = "{" + Service_ID + "}event";

			if(version==0)
			{
				Major = 1;
				Minor = 0;
			}
			else
			{
				DText TempNum = new DText();
				Major = int.Parse(TempNum.FIELD(version.ToString(),".",1));
				if(TempNum.DCOUNT(version.ToString(),".") ==2)
				{
					Minor = int.Parse(TempNum.FIELD(version.ToString(),".",2));
				}
				else
				{
					Minor = 0;
				}
			}
		}
		public String Version
		{
			get
			{
				return(Major.ToString() + "." + Minor.ToString());
			}
		}
	}
}
