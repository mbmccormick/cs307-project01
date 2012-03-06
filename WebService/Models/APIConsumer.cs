﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;
using System.Runtime.Serialization;
using WebService.Common;

namespace WebService.Models
{
    [DataContract]
    public class APIConsumer
    {
        [DataMember(Order = 0)]
        [BsonId(IdGenerator = typeof(StringObjectIdGenerator))]
        public string ID
        {
            get;
            set;
        }

        [DataMember(Order = 1)]
        public string EmailAddress
        {
            get;
            set;
        }

        [DataMember(Order = 2)]
        public string Key
        {
            get
            {
                return this.ID;
            }
        }

        public DateTime CreatedDate
        {
            get;
            set;
        }

        [DataMember(Name = "CreatedDate", Order = 3)]
        public int JsonCreatedDate
        {
            get
            {
                return Utilities.ConvertToUnixTime(this.CreatedDate);
            }

            set
            {
                this.CreatedDate = Utilities.ConvertFromUnixTime(value);
            }
        }
    }
}