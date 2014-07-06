using System;
using System.Collections.Generic;
using System.Linq;
using mjjames.AdminSystem.classes;
using System.Configuration;
using mjjames.AdminSystem.DataEntities;

namespace mjjames.AdminSystem.Repositories
{
	public class KeyValueRepository
	{
		private readonly ILogger _logger = new Logger("KeyValueRepository");
		private readonly DataContexts.AdminDataContext _admin = new DataContexts.AdminDataContext(ConfigurationManager.ConnectionStrings["ourDatabase"].ConnectionString);


		public string GetKeyValue(string lookupID, int linkKey, string linktypeID)
		{
			var data = (from kv in _admin.keyvalues
			            where kv.lookup.lookup_id.ToLower() == lookupID.ToLower()
			                  && kv.link_fkey == linkKey
			                  && kv.lookup1.lookup_id.ToLower() == linktypeID.ToLower()
			            select kv).FirstOrDefault();
			return data != null ? data.value : String.Empty;
		}

		public void UpdateKeyValues(IEnumerable<KeyValueData> data)
		{
			//first we must try and pull out existing key values
			//we actually stash them away in a dictionary,the key made up from the values we pass in as it's easier to match them up
			var updateData = (from kv in _admin.keyvalues
			                 where data.Select(k => k.LinkKey).Contains(kv.link_fkey) 
							 	&& data.Select(k => k.LinkTypeID.ToLower()).Contains(kv.lookup1.lookup_id.ToLower())
			                       && data.Select(k => k.LookupID.ToLower()).Contains(kv.lookup.lookup_id.ToLower())
			                 select kv).ToDictionary(kv => String.Format("{0}-{1}-{2}", kv.link_fkey, kv.lookup.lookup_id.ToLower(), kv.lookup1.lookup_id.ToLower()),
												 kv => kv);
			
			var dataToAdd = new List<KeyValueData>();

			//then update the values and write them away
			foreach (var keyvalue in data)
			{
				var key = String.Format("{0}-{1}-{2}", keyvalue.LinkKey, keyvalue.LookupID.ToLower(), keyvalue.LinkTypeID.ToLower());
				//if we dont contain the data it must be a new item so save it
				if(!updateData.ContainsKey(key))
				{
					dataToAdd.Add(keyvalue);
					continue;
				}
				updateData[key].value = keyvalue.Value;
			}

			//save our updates back to the DB
			_admin.SubmitChanges();
			
			// next remaining data must be inserted 
			AddKeyValues(dataToAdd) ; 
		}

		public void AddKeyValues(IEnumerable<KeyValueData> data)
		{
			//bit of explanation of this crazy stuff
			//we have to lookup the keys of the lookup id's, however rather than look this up for each of them we should look all of them up
			//stash them in a lookup and then use them upon insert

			var lookupIDs = data.Select(k => k.LinkTypeID.ToLower()).Union(data.Select(k => k.LookupID.ToLower())).ToList();
			var lookupDict = (from l in _admin.lookups
			                 where lookupIDs.Contains(l.lookup_id.ToLower())
			                 select new
			                        	{
			                        		l.lookup_id,
			                        		l.lookup_key
			                        	}).ToDictionary(k => k.lookup_id.ToLower(), k => k.lookup_key);

			//now build new keyvalue objects to actually insert into the db 

			var dataToInsert = data.Select(k => new keyvalue
			                                    	{
			                                    		link_fkey = k.LinkKey,
			                                    		value = k.Value,
			                                    		key_lookup = lookupDict[k.LinkTypeID.ToLower()],
			                                    		link_lookup = lookupDict[k.LookupID.ToLower()]
			                                    	});

			_admin.keyvalues.InsertAllOnSubmit(dataToInsert); 
			_admin.SubmitChanges();
		}
	}

	public struct KeyValueData
	{
		public string LookupID { get; set;}
		public string LinkTypeID { get; set;}
		public int LinkKey { get; set;}
		public string Value { get; set;}
	}
}