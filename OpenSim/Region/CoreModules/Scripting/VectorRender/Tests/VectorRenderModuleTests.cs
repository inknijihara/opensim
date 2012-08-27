/*
 * Copyright (c) Contributors, http://opensimulator.org/
 * See CONTRIBUTORS.TXT for a full list of copyright holders.
 *
 * Redistribution and use in source and binary forms, with or without
 * modification, are permitted provided that the following conditions are met:
 *     * Redistributions of source code must retain the above copyright
 *       notice, this list of conditions and the following disclaimer.
 *     * Redistributions in binary form must reproduce the above copyright
 *       notice, this list of conditions and the following disclaimer in the
 *       documentation and/or other materials provided with the distribution.
 *     * Neither the name of the OpenSimulator Project nor the
 *       names of its contributors may be used to endorse or promote products
 *       derived from this software without specific prior written permission.
 *
 * THIS SOFTWARE IS PROVIDED BY THE DEVELOPERS ``AS IS'' AND ANY
 * EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
 * WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
 * DISCLAIMED. IN NO EVENT SHALL THE CONTRIBUTORS BE LIABLE FOR ANY
 * DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
 * (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
 * LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
 * ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
 * (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
 * SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading;
using log4net.Config;
using NUnit.Framework;
using OpenMetaverse;
using OpenMetaverse.Assets;
using OpenSim.Framework;
using OpenSim.Region.CoreModules.Scripting.DynamicTexture;
using OpenSim.Region.CoreModules.Scripting.VectorRender;
using OpenSim.Region.Framework.Scenes;
using OpenSim.Region.Framework.Scenes.Serialization;
using OpenSim.Tests.Common;
using OpenSim.Tests.Common.Mock;

namespace OpenSim.Region.CoreModules.Scripting.VectorRender.Tests
{
    [TestFixture]
    public class VectorRenderModuleTests : OpenSimTestCase
    {
        Scene m_scene;
        DynamicTextureModule m_dtm;
        VectorRenderModule m_vrm;

        [SetUp]
        public void SetUp()
        {
            m_scene = new SceneHelpers().SetupScene();
            m_dtm = new DynamicTextureModule();
            m_vrm = new VectorRenderModule();
            SceneHelpers.SetupSceneModules(m_scene, m_dtm, m_vrm);
        }

        [Test]
        public void TestDraw()
        {
            TestHelpers.InMethod();

            SceneObjectGroup so = SceneHelpers.AddSceneObject(m_scene);
            UUID originalTextureID = so.RootPart.Shape.Textures.GetFace(0).TextureID;

            m_dtm.AddDynamicTextureData(
                m_scene.RegionInfo.RegionID,
                so.UUID,
                m_vrm.GetContentType(),
                "PenColour BLACK; MoveTo 40,220; FontSize 32; Text Hello World;",
                "",
                0);

            Assert.That(originalTextureID, Is.Not.EqualTo(so.RootPart.Shape.Textures.GetFace(0).TextureID));
        }

        [Test]
        public void TestRepeatDraw()
        {
            TestHelpers.InMethod();

            string dtText = "PenColour BLACK; MoveTo 40,220; FontSize 32; Text Hello World;";

            SceneObjectGroup so = SceneHelpers.AddSceneObject(m_scene);

            m_dtm.AddDynamicTextureData(
                m_scene.RegionInfo.RegionID,
                so.UUID,
                m_vrm.GetContentType(),
                dtText,
                "",
                0);

            UUID firstDynamicTextureID = so.RootPart.Shape.Textures.GetFace(0).TextureID;

            m_dtm.AddDynamicTextureData(
                m_scene.RegionInfo.RegionID,
                so.UUID,
                m_vrm.GetContentType(),
                dtText,
                "",
                0);

            Assert.That(firstDynamicTextureID, Is.Not.EqualTo(so.RootPart.Shape.Textures.GetFace(0).TextureID));
        }

        [Test]
        public void TestRepeatDrawContainingImage()
        {
            TestHelpers.InMethod();

            string dtText
                = "PenColour BLACK; MoveTo 40,220; FontSize 32; Text Hello World; Image http://localhost/shouldnotexist.png";

            SceneObjectGroup so = SceneHelpers.AddSceneObject(m_scene);

            m_dtm.AddDynamicTextureData(
                m_scene.RegionInfo.RegionID,
                so.UUID,
                m_vrm.GetContentType(),
                dtText,
                "",
                0);

            UUID firstDynamicTextureID = so.RootPart.Shape.Textures.GetFace(0).TextureID;

            m_dtm.AddDynamicTextureData(
                m_scene.RegionInfo.RegionID,
                so.UUID,
                m_vrm.GetContentType(),
                dtText,
                "",
                0);

            Assert.That(firstDynamicTextureID, Is.Not.EqualTo(so.RootPart.Shape.Textures.GetFace(0).TextureID));
        }
    }
}