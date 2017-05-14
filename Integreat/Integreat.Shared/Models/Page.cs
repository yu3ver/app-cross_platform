﻿using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Integreat.Shared.Utilities;
using Newtonsoft.Json.Linq;
using System.Diagnostics;

namespace Integreat.Shared.Models {
    public class Page {

        [JsonProperty("parent")]
        public string ParentJsonId { get; set; }

        [JsonProperty("permalink")]
        public PagePermalinks Permalinks { get; set; }

        [JsonProperty("parentId")]
        public string ParentId { get; set; }

        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("modified_gmt")]
        [JsonConverter(typeof(DateConverter))]
        public DateTime Modified { get; set; }

        [JsonProperty("excerpt")]
        public string Description { get; set; }

        [JsonProperty("content")]
        public string Content { get; set; }

        [JsonProperty("order")]
        public int Order { get; set; }

        [JsonProperty("thumbnail")]
        public string Thumbnail { get; set; }


        [JsonProperty("author")]
        public Author Author { get; set; }

        [JsonProperty("available_languages")]
        [JsonConverter(typeof(AvailableLanguageCollectionConverter))]
        public List<AvailableLanguage> AvailableLanguages { get; set; }

        public string PrimaryKey { get; set; }


        internal bool Find(string searchText) {
            var pageString = (Title ?? "") + (Description ?? "");
            return pageString.ToLower().Contains((searchText ?? "").ToLower());
        }

        public static string GenerateKey(object id, Location location, Language language) {
            if (location == null || language == null) return "";
            return id + "_" + language.Id + "_" + location.Id;
        }
    }

    internal class DateConverter : JsonConverter {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) {
            var dt = value as DateTime? ?? new DateTime();
            writer.WriteValue(dt.ToRestAcceptableString());
        }

        public override bool CanConvert(Type type) {
            return Reflections.IsAssignableFrom(typeof(DateTime), type);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue,
                                              JsonSerializer serializer) {
            var readerValue = reader.Value.ToString();
            return readerValue.DateTimeFromRestString();
        }
    }

    internal class AvailableLanguageCollectionConverter : JsonConverter {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) {
            var asList = value as List<AvailableLanguage>;
            if (asList == null) return;
            var props = (from lang in asList
                         select new JProperty(lang.LanguageId, lang.OtherPageId));
            var jObject = new JObject(props);
            serializer.Serialize(writer, jObject);
        }

        public override bool CanConvert(Type type) {
            return Reflections.IsAssignableFrom(typeof(List<AvailableLanguage>), type);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue,
            JsonSerializer serializer) {
            try {
                var dict2 = serializer.Deserialize(reader) as JObject;
                return dict2 == null ? new List<AvailableLanguage>() : (from jToken in dict2?.Properties()
                                                                        select new AvailableLanguage(jToken.Name, jToken.Value.ToString())).ToList();
            } catch (Exception e) {
                Debug.WriteLine(e);
                serializer.Deserialize(reader);
                return new List<AvailableLanguage>();
            }
        }

    }
}

