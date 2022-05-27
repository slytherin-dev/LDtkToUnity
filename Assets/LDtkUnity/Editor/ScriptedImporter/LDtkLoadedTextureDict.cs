﻿using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;

namespace LDtkUnity.Editor
{
    /// <summary>
    /// relativePath, Texture<br/>
    /// This is used so we don't load textures more than once when creating import artifacts.<br/>
    /// It's structured like this with relative paths as keys so that even if a texture is used as both a background and tile set, then it's still only loaded once
    /// </summary>
    internal class LDtkLoadedTextureDict
    {
        private readonly string _assetPath;
        
        private readonly Dictionary<string, Texture2D> _dict = new Dictionary<string, Texture2D>();
        private readonly HashSet<string> _attemptedFailures = new HashSet<string>();

        public LDtkLoadedTextureDict(string assetPath)
        {
            _assetPath = assetPath;
        }

        public IEnumerable<Texture2D> Textures => _dict.Values;

        public void LoadAll(LdtkJson json)
        {
            TilesetDefinition[] defs = json.Defs.Tilesets;
            
            //acquire tile set textures 
            foreach (TilesetDefinition def in defs)
            {
                Profiler.BeginSample(def.Identifier);
                TryAdd(def, def.RelPath, LoadTilesetTex);
                Profiler.EndSample();
            }

            //acquire level backgrounds
            foreach (World world in json.UnityWorlds)
            {
                foreach (Level level in world.Levels) //get textures references from level backgrounds. gather them all
                {
                    Profiler.BeginSample(level.Identifier);
                    TryAdd(level, level.BgRelPath, LoadLevelBackground);
                    Profiler.EndSample();
                }
            }
        }
        
        public Texture2D Get(string relPath)
        {
            if (string.IsNullOrEmpty(relPath))
            {
                return null;
            }

            if (!_dict.ContainsKey(relPath))
            {
                //LDtkDebug.LogError($"Failed getting texture from {_assetPath}: {relPath}, the dictionary didn't contain the key when trying to get it.");
                return null;
            }

            Texture2D tex = _dict[relPath];
            if (tex == null)
            {
                //LDtkDebug.LogError($"Failed getting texture for {_assetPath}: {relPath}, asset was null");
                return null;
            }
            
            return tex;
        }

        
        private delegate Texture2D ExternalLoadMethod<in T>(T data);
        private Texture2D TryAdd<T>(T data, string relPath, ExternalLoadMethod<T> action)
        {
            if (string.IsNullOrEmpty(relPath))
            {
                return null;
            }

            if (_dict.ContainsKey(relPath))
            {
                return _dict[relPath];
            }
            
            if (_attemptedFailures.Contains(relPath))
            {
                //LDtkDebug.LogError($"Failed loading texture from {_assetPath}: {relPath}");
                return null;
            }

            Texture2D tex = action.Invoke(data);
            if (tex != null)
            {
                _dict.Add(relPath, tex);
                return tex;
            }
            
            _attemptedFailures.Add(relPath);
            //LDtkDebug.LogError($"Failed loading texture from {_assetPath}: {relPath}");
            return null;
        }
        
        private Texture2D LoadTilesetTex(TilesetDefinition def)
        {
            LDtkRelativeGetterTilesetTexture getter = new LDtkRelativeGetterTilesetTexture();
            return getter.GetRelativeAsset(def, _assetPath);
        }
        private Texture2D LoadLevelBackground(Level def)
        {
            LDtkRelativeGetterLevelBackground getter = new LDtkRelativeGetterLevelBackground();
            return getter.GetRelativeAsset(def, _assetPath);
        }
    }
}