﻿using System;
using LDtkUnity.Runtime.Data;
using Newtonsoft.Json;
using UnityEngine;

namespace LDtkUnity.Runtime.Tools
{
    public static class LDtkToolProjectLoader
    {
        public static LDtkDataProject DeserializeProject(string json)
        {
            try
            {
                JsonSerializerSettings s = new JsonSerializerSettings()
                {
                    NullValueHandling = NullValueHandling.Ignore,
                };
                
                LDtkDataProject project = JsonConvert.DeserializeObject<LDtkDataProject>(json, s);
                return project;
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                throw;
            }
        }

        public static bool IsValidJson(string json)
        {
            try
            {
                JsonSerializerSettings s = new JsonSerializerSettings()
                {
                    NullValueHandling = NullValueHandling.Ignore,
                };
                JsonConvert.DeserializeObject<LDtkDataProject>(json, s);
            }
            catch (Exception e)
            {
                return false;
            }

            return true;
        }
    }
}