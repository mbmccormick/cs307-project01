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
    public class FavoritedPicture
    {
        [DataMember(Order = 0)]
        [BsonId(IdGenerator = typeof(StringObjectIdGenerator))]
        public string ID
        {
            get;
            set;
        }

        [DataMember(Order = 1)]
        public string UserID
        {
            get;
            set;
        }

        [DataMember(Order = 2)]
        public string PictureID
        {
            get;
            set;
        }

        [DataMember(Order = 3)]
        public int CreatedDate
        {
            get;
            set;
        }

        [BsonIgnore]
        public DateTime FriendlyCreatedDate
        {
            get
            {
                return Utilities.ConvertFromUnixTime(this.CreatedDate);
            }

            set
            {
                this.CreatedDate = Utilities.ConvertToUnixTime(value);
            }
        }
    }
}