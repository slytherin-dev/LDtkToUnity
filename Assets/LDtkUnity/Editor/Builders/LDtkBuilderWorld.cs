﻿using UnityEngine;

namespace LDtkUnity.Editor
{
    internal class LDtkBuilderWorld
    {
        private readonly LDtkProjectImporter _importer;
        private readonly LdtkJson _json;
        private readonly World _world;
        private readonly LDtkPostProcessorCache _postProcess;
        private readonly LDtkLinearLevelVector _linearVector = new LDtkLinearLevelVector();

        private GameObject _worldObject;

        public LDtkBuilderWorld(LDtkProjectImporter importer, LdtkJson json, World world, LDtkPostProcessorCache postProcess)
        {
            _importer = importer;
            _json = json;
            _world = world;
            _postProcess = postProcess;
        }
        
        public GameObject BuildWorld()
        {
            CreateWorldObject();
            
            foreach (Level lvl in _world.Levels)
            {
                WorldLayout layout = _world.WorldLayout.HasValue ? _world.WorldLayout.Value : WorldLayout.Free;
                LDtkBuilderLevel levelBuilder = new LDtkBuilderLevel(_importer, _json, layout, lvl, _postProcess, _linearVector);
                
                GameObject levelObj = levelBuilder.BuildLevel();
                levelObj.transform.SetParent(_worldObject.transform);
            }

            return _worldObject;
        }
        
        private void CreateWorldObject()
        {
            _worldObject = new GameObject(_world.Identifier);

            LDtkIid iid = _worldObject.AddComponent<LDtkIid>();
            iid.SetIid(_world);

            if (_importer.DeparentInRuntime)
            {
                _worldObject.AddComponent<LDtkDetachChildren>();
            }
        }
    }
}