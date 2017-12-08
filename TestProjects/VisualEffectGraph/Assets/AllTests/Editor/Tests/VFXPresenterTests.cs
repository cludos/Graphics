using System;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.VFX;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.VFX.UI;
using System.IO;
using UnityEditor.VFX.Block.Test;

namespace UnityEditor.VFX.Test
{
    [TestFixture]
    public class VFXPresentersTests
    {
        VFXViewPresenter m_ViewPresenter;

        const string testAssetName = "Assets/TmpTests/VFXGraph1.asset";

        private int m_StartUndoGroupId;

        [SetUp]
        public void CreateTestAsset()
        {
            VFXAsset asset = new VFXAsset();

            var directoryPath = Path.GetDirectoryName(testAssetName);
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            AssetDatabase.CreateAsset(asset, testAssetName);

            m_ViewPresenter = VFXViewPresenter.Manager.GetPresenter(asset);

            m_StartUndoGroupId = Undo.GetCurrentGroup();
        }

        [TearDown]
        public void DestroyTestAsset()
        {
            Undo.RevertAllDownToGroup(m_StartUndoGroupId);
            AssetDatabase.DeleteAsset(testAssetName);
        }

        [Test]
        public void CreateAllInitializeBlocks()
        {
            var initContextDesc = VFXLibrary.GetContexts().Where(t => t.name == "Initialize").First();

            var newContext = m_ViewPresenter.AddVFXContext(new Vector2(100, 100), initContextDesc);

            Assert.AreEqual(m_ViewPresenter.allChildren.Where(t => t is VFXContextPresenter).Count(), 1);

            var contextPresenter = m_ViewPresenter.allChildren.Where(t => t is VFXContextPresenter).First() as VFXContextPresenter;

            Assert.AreEqual(contextPresenter.model, newContext);

            // Adding every block compatible with an init context
            foreach (var block in VFXLibrary.GetBlocks().Where(t => t.AcceptParent(newContext)))
            {
                var newBlock = block.CreateInstance();
                contextPresenter.AddBlock(0, newBlock);

                Assert.AreEqual(contextPresenter.blockPresenters.Where(t => t.block == newBlock).Count(), 1);

                var blockPresenter = contextPresenter.blockPresenters.Where(t => t.block == newBlock).First() as VFXBlockPresenter;

                Assert.NotNull(blockPresenter);
            }
        }

        [Test]
        public void CreateAllUpdateBlocks()
        {
            var initContextDesc = VFXLibrary.GetContexts().Where(t => t.name == "Update").First();

            var newContext = m_ViewPresenter.AddVFXContext(new Vector2(100, 100), initContextDesc);

            Assert.AreEqual(m_ViewPresenter.allChildren.Where(t => t is VFXContextPresenter).Count(), 1);

            var contextPresenter = m_ViewPresenter.allChildren.Where(t => t is VFXContextPresenter).First() as VFXContextPresenter;

            Assert.AreEqual(contextPresenter.model, newContext);

            // Adding every block compatible with an init context
            foreach (var block in VFXLibrary.GetBlocks().Where(t => t.AcceptParent(newContext)))
            {
                var newBlock = block.CreateInstance();
                contextPresenter.AddBlock(0, newBlock);

                Assert.AreEqual(contextPresenter.blockPresenters.Where(t => t.block == newBlock).Count(), 1);

                var blockPresenter = contextPresenter.blockPresenters.Where(t => t.block == newBlock).First() as VFXBlockPresenter;

                Assert.NotNull(blockPresenter);
            }
        }

        [Test]
        public void CreateAllOutputBlocks()
        {
            var initContextDesc = VFXLibrary.GetContexts().Where(t => t.name.Contains("Output")).First();

            var newContext = m_ViewPresenter.AddVFXContext(new Vector2(100, 100), initContextDesc);

            Assert.AreEqual(m_ViewPresenter.allChildren.Where(t => t is VFXContextPresenter).Count(), 1);

            var contextPresenter = m_ViewPresenter.allChildren.Where(t => t is VFXContextPresenter).First() as VFXContextPresenter;

            Assert.AreEqual(contextPresenter.model, newContext);

            // Adding every block compatible with an init context
            foreach (var block in VFXLibrary.GetBlocks().Where(t => t.AcceptParent(newContext)))
            {
                var newBlock = block.CreateInstance();
                contextPresenter.AddBlock(0, newBlock);

                Assert.AreEqual(contextPresenter.blockPresenters.Where(t => t.block == newBlock).Count(), 1);

                var blockPresenter = contextPresenter.blockPresenters.Where(t => t.block == newBlock).First() as VFXBlockPresenter;

                Assert.NotNull(blockPresenter);
            }
        }

        [Test]
        public void ExpandRetractAndSetPropertyValue()
        {
            var initContextDesc = VFXLibrary.GetContexts().Where(t => t.name == "Initialize").First();

            var newContext = m_ViewPresenter.AddVFXContext(new Vector2(100, 100), initContextDesc);

            Assert.AreEqual(m_ViewPresenter.allChildren.Where(t => t is VFXContextPresenter).Count(), 1);

            var contextPresenter = m_ViewPresenter.allChildren.Where(t => t is VFXContextPresenter).First() as VFXContextPresenter;

            Assert.AreEqual(contextPresenter.model, newContext);

            // Adding every block compatible with an init context

            var block = VFXLibrary.GetBlocks().Where(t => t.name == "Test").First();

            var newBlock = block.CreateInstance();
            contextPresenter.AddBlock(0, newBlock);

            Assert.IsTrue(newBlock is AllType);

            Assert.AreEqual(contextPresenter.blockPresenters.Where(t => t.block == newBlock).Count(), 1);

            var blockPresenter = contextPresenter.blockPresenters.Where(t => t.block == newBlock).First() as VFXBlockPresenter;

            Assert.NotNull(blockPresenter);

            Assert.NotZero(blockPresenter.inputPorts.Where(t => t is VFXContextDataInputAnchorPresenter && (t as VFXContextDataInputAnchorPresenter).name == "aVector3").Count());

            VFXSlot slot = blockPresenter.block.inputSlots.First(t => t.name == "aVector3");


            var aVector3Presenter = blockPresenter.inputPorts.Where(t => t is VFXContextDataInputAnchorPresenter && (t as VFXContextDataInputAnchorPresenter).name == "aVector3").First() as VFXContextDataInputAnchorPresenter;

            Assert.AreEqual(blockPresenter.inputPorts.Where(t => t is VFXContextDataInputAnchorPresenter && (t as VFXContextDataInputAnchorPresenter).path == "aVector3.x").Count(), 1);
            Assert.AreEqual(blockPresenter.inputPorts.Where(t => t is VFXContextDataInputAnchorPresenter && (t as VFXContextDataInputAnchorPresenter).path == "aVector3.y").Count(), 1);
            Assert.AreEqual(blockPresenter.inputPorts.Where(t => t is VFXContextDataInputAnchorPresenter && (t as VFXContextDataInputAnchorPresenter).path == "aVector3.z").Count(), 1);

            aVector3Presenter.ExpandPath();

            Assert.AreEqual(blockPresenter.inputPorts.Where(t => t is VFXContextDataInputAnchorPresenter && (t as VFXContextDataInputAnchorPresenter).path == "aVector3.x").Count(), 1);
            Assert.AreEqual(blockPresenter.inputPorts.Where(t => t is VFXContextDataInputAnchorPresenter && (t as VFXContextDataInputAnchorPresenter).path == "aVector3.y").Count(), 1);
            Assert.AreEqual(blockPresenter.inputPorts.Where(t => t is VFXContextDataInputAnchorPresenter && (t as VFXContextDataInputAnchorPresenter).path == "aVector3.z").Count(), 1);


            aVector3Presenter.RetractPath();

            Assert.AreEqual(blockPresenter.inputPorts.Where(t => t is VFXContextDataInputAnchorPresenter && (t as VFXContextDataInputAnchorPresenter).path == "aVector3.x").Count(), 1);
            Assert.AreEqual(blockPresenter.inputPorts.Where(t => t is VFXContextDataInputAnchorPresenter && (t as VFXContextDataInputAnchorPresenter).path == "aVector3.y").Count(), 1);
            Assert.AreEqual(blockPresenter.inputPorts.Where(t => t is VFXContextDataInputAnchorPresenter && (t as VFXContextDataInputAnchorPresenter).path == "aVector3.z").Count(), 1);


            aVector3Presenter.SetPropertyValue(new Vector3(1.2f, 3.4f, 5.6f));

            Assert.AreEqual(slot.value, new Vector3(1.2f, 3.4f, 5.6f));

            aVector3Presenter.ExpandPath();

            var vector3yPresenter = blockPresenter.inputPorts.Where(t => t is VFXContextDataInputAnchorPresenter && (t as VFXContextDataInputAnchorPresenter).path == "aVector3.y").First() as VFXContextDataInputAnchorPresenter;

            vector3yPresenter.SetPropertyValue(7.8f);

            Assert.AreEqual(slot.value, new Vector3(1.2f, 7.8f, 5.6f));
        }

        [Test]
        public void CascadedOperatorAdd()
        {
            Func<IVFXSlotContainer, VFXSlotContainerPresenter> fnFindPresenter = delegate(IVFXSlotContainer slotContainer)
                {
                    var allPresenter = m_ViewPresenter.allChildren.OfType<VFXSlotContainerPresenter>();
                    return allPresenter.FirstOrDefault(o => o.slotContainer == slotContainer);
                };

            var vector2Desc = VFXLibrary.GetParameters().FirstOrDefault(o => o.name == "Vector2");
            var vector2 = m_ViewPresenter.AddVFXParameter(new Vector2(-100, -100), vector2Desc);

            var addDesc = VFXLibrary.GetOperators().FirstOrDefault(o => o.name == "Add");
            var add = m_ViewPresenter.AddVFXOperator(new Vector2(100, 100), addDesc);

            var absDesc = VFXLibrary.GetOperators().FirstOrDefault(o => o.name == "Absolute");
            var abs = m_ViewPresenter.AddVFXOperator(new Vector2(100, 100), absDesc);

            var absPresenter = fnFindPresenter(abs);
            var addPresenter = fnFindPresenter(add);
            var edgePresenter = VFXDataEdgePresenter.CreateInstance<VFXDataEdgePresenter>();
            edgePresenter.Init(addPresenter.outputPorts.First(), absPresenter.inputPorts.First());
            m_ViewPresenter.AddElement(edgePresenter);
            Assert.AreEqual(VFXValueType.kFloat, abs.outputSlots[0].GetExpression().valueType);

            var vector2Presenter = fnFindPresenter(vector2);
            for (int i = 0; i < 4; ++i)
            {
                edgePresenter = VFXDataEdgePresenter.CreateInstance<VFXDataEdgePresenter>();
                edgePresenter.Init(vector2Presenter.outputPorts.First(), addPresenter.inputPorts.First());
                m_ViewPresenter.AddElement(edgePresenter);
            }

            Assert.AreEqual(VFXValueType.kFloat2, add.outputSlots[0].GetExpression().valueType);
            Assert.AreEqual(VFXValueType.kFloat2, abs.outputSlots[0].GetExpression().valueType);

            m_ViewPresenter.RemoveElement(addPresenter);
            Assert.AreEqual(VFXValueType.kFloat, abs.outputSlots[0].GetExpression().valueType);
        }

        [Test]
        public void AppendOperator()
        {
            Action fnResync = delegate()
                {
                    m_ViewPresenter.ForceReload();
                };

            Func<IVFXSlotContainer, VFXSlotContainerPresenter> fnFindPresenter = delegate(IVFXSlotContainer slotContainer)
                {
                    var allPresenter = m_ViewPresenter.allChildren.OfType<VFXSlotContainerPresenter>();
                    return allPresenter.FirstOrDefault(o => o.slotContainer == slotContainer);
                };

            var absDesc = VFXLibrary.GetOperators().FirstOrDefault(o => o.name == "Absolute");
            var abs = m_ViewPresenter.AddVFXOperator(new Vector2(100, 100), absDesc); fnResync();

            var cosDesc = VFXLibrary.GetOperators().FirstOrDefault(o => o.name == "Cosine");
            var cos = m_ViewPresenter.AddVFXOperator(new Vector2(200, 100), cosDesc); fnResync();

            var appendDesc = VFXLibrary.GetOperators().FirstOrDefault(o => o.name == "AppendVector");
            var append = m_ViewPresenter.AddVFXOperator(new Vector2(300, 100), appendDesc); fnResync();

            var edgePresenterCos = VFXDataEdgePresenter.CreateInstance<VFXDataEdgePresenter>();
            edgePresenterCos.Init(fnFindPresenter(append).outputPorts.First(), fnFindPresenter(cos).inputPorts.First());
            m_ViewPresenter.AddElement(edgePresenterCos); fnResync();

            var edgePresenterAppend_A = VFXDataEdgePresenter.CreateInstance<VFXDataEdgePresenter>();
            edgePresenterAppend_A.Init(fnFindPresenter(abs).outputPorts.First(), fnFindPresenter(append).inputPorts.First());
            m_ViewPresenter.AddElement(edgePresenterAppend_A); fnResync();

            var edgePresenterAppend_B = VFXDataEdgePresenter.CreateInstance<VFXDataEdgePresenter>();
            edgePresenterAppend_B.Init(fnFindPresenter(abs).outputPorts.First(), fnFindPresenter(append).inputPorts[1]);
            m_ViewPresenter.AddElement(edgePresenterAppend_B); fnResync();

            var edgePresenterAppend_C = VFXDataEdgePresenter.CreateInstance<VFXDataEdgePresenter>();
            edgePresenterAppend_C.Init(fnFindPresenter(abs).outputPorts.First(), fnFindPresenter(append).inputPorts[2]);
            m_ViewPresenter.AddElement(edgePresenterAppend_C); fnResync();

            var edgePresenterAppend_D = VFXDataEdgePresenter.CreateInstance<VFXDataEdgePresenter>();
            edgePresenterAppend_D.Init(fnFindPresenter(abs).outputPorts.First(), fnFindPresenter(append).inputPorts[3]);
            m_ViewPresenter.AddElement(edgePresenterAppend_D); fnResync();
        }

        [Test]
        public void UndoRedoCollapseSlot()
        {
            Undo.IncrementCurrentGroup();
            var crossDesc = VFXLibrary.GetOperators().FirstOrDefault(o => o.name.Contains("Cross"));
            var cross = m_ViewPresenter.AddVFXOperator(new Vector2(0, 0), crossDesc);

            foreach (var slot in cross.inputSlots.Concat(cross.outputSlots))
            {
                Undo.IncrementCurrentGroup();
                Assert.IsTrue(slot.collapsed);
                slot.collapsed = false;
            }

            var totalSlotCount = cross.inputSlots.Concat(cross.outputSlots).Count();
            for (int step = 1; step < totalSlotCount; step++)
            {
                Undo.PerformUndo();
                var vfxOperatorPresenter = m_ViewPresenter.allChildren.OfType<VFXOperatorPresenter>().FirstOrDefault();
                Assert.IsNotNull(vfxOperatorPresenter);

                var slots = vfxOperatorPresenter.Operator.inputSlots.Concat(vfxOperatorPresenter.Operator.outputSlots).Reverse();
                for (int i = 0; i < totalSlotCount; ++i)
                {
                    var slot = slots.ElementAt(i);
                    Assert.AreEqual(i < step, slot.collapsed);
                }
            }

            for (int step = 1; step < totalSlotCount; step++)
            {
                Undo.PerformRedo();
                var vfxOperatorPresenter = m_ViewPresenter.allChildren.OfType<VFXOperatorPresenter>().FirstOrDefault();
                Assert.IsNotNull(vfxOperatorPresenter);

                var slots = vfxOperatorPresenter.Operator.inputSlots.Concat(vfxOperatorPresenter.Operator.outputSlots);
                for (int i = 0; i < totalSlotCount; ++i)
                {
                    var slot = slots.ElementAt(i);
                    Assert.AreEqual(i > step, slot.collapsed);
                }
            }
        }

        [Test]
        public void UndoRedoMoveOperator()
        {
            Undo.IncrementCurrentGroup();
            var absDesc = VFXLibrary.GetOperators().FirstOrDefault(o => o.name == "Absolute");
            var abs = m_ViewPresenter.AddVFXOperator(new Vector2(0, 0), absDesc);

            var positions = new[] { new Vector2(1, 1), new Vector2(2, 2), new Vector2(3, 3), new Vector2(4, 4) };
            foreach (var position in positions)
            {
                Undo.IncrementCurrentGroup();
                abs.position = position;
            }

            Func<Type, VFXSlotContainerPresenter> fnFindPresenter = delegate(Type type)
                {
                    var allPresenter = m_ViewPresenter.allChildren.OfType<VFXSlotContainerPresenter>();
                    return allPresenter.FirstOrDefault(o => type.IsInstanceOfType(o.slotContainer));
                };

            for (int i = 0; i < positions.Length; ++i)
            {
                var currentAbs = fnFindPresenter(typeof(VFXOperatorAbsolute));
                Assert.IsNotNull(currentAbs);
                Assert.AreEqual(positions[positions.Length - i - 1].x, currentAbs.model.position.x);
                Assert.AreEqual(positions[positions.Length - i - 1].y, currentAbs.model.position.y);
                Undo.PerformUndo();
            }

            for (int i = 0; i < positions.Length; ++i)
            {
                Undo.PerformRedo();
                var currentAbs = fnFindPresenter(typeof(VFXOperatorAbsolute));
                Assert.IsNotNull(currentAbs);
                Assert.AreEqual(positions[i].x, currentAbs.model.position.x);
                Assert.AreEqual(positions[i].y, currentAbs.model.position.y);
            }
        }

        [Test]
        public void UndoRedoAddOperator()
        {
            Func<VFXSlotContainerPresenter[]> fnAllOperatorPresenter = delegate()
                {
                    var allPresenter = m_ViewPresenter.allChildren.OfType<VFXSlotContainerPresenter>();
                    return allPresenter.OfType<VFXOperatorPresenter>().ToArray();
                };


            Action fnTestShouldExist = delegate()
                {
                    var allOperatorPresenter = fnAllOperatorPresenter();
                    Assert.AreEqual(1, allOperatorPresenter.Length);
                    Assert.IsInstanceOf(typeof(VFXOperatorAbsolute), allOperatorPresenter[0].model);
                };

            Action fnTestShouldNotExist = delegate()
                {
                    var allOperatorPresenter = fnAllOperatorPresenter();
                    Assert.AreEqual(0, allOperatorPresenter.Length);
                };

            Undo.IncrementCurrentGroup();
            var absDesc = VFXLibrary.GetOperators().FirstOrDefault(o => o.name == "Absolute");
            var abs = m_ViewPresenter.AddVFXOperator(new Vector2(0, 0), absDesc);

            fnTestShouldExist();
            Undo.PerformUndo();
            fnTestShouldNotExist();
            Undo.PerformRedo();
            fnTestShouldExist();

            Undo.IncrementCurrentGroup();
            m_ViewPresenter.RemoveElement(fnAllOperatorPresenter()[0]);
            fnTestShouldNotExist();
            Undo.PerformUndo();
            fnTestShouldExist();
            Undo.PerformRedo();
            fnTestShouldNotExist();
        }

        [Test]
        public void UndoRedoOperatorLinkSimple()
        {
            Func<Type, VFXSlotContainerPresenter> fnFindPresenter = delegate(Type type)
                {
                    var allPresenter = m_ViewPresenter.allChildren.OfType<VFXSlotContainerPresenter>();
                    return allPresenter.FirstOrDefault(o => type.IsInstanceOfType(o.slotContainer));
                };

            var cosDesc = VFXLibrary.GetOperators().FirstOrDefault(o => o.name == "Cosine");
            var sinDesc = VFXLibrary.GetOperators().FirstOrDefault(o => o.name == "Sine");
            Undo.IncrementCurrentGroup();
            var cos = m_ViewPresenter.AddVFXOperator(new Vector2(0, 0), cosDesc);
            Undo.IncrementCurrentGroup();
            var sin = m_ViewPresenter.AddVFXOperator(new Vector2(1, 1), sinDesc);
            var cosPresenter = fnFindPresenter(typeof(VFXOperatorCosine));
            var sinPresenter = fnFindPresenter(typeof(VFXOperatorSine));

            Func<int> fnCountEdge = delegate()
                {
                    return m_ViewPresenter.allChildren.OfType<VFXDataEdgePresenter>().Count();
                };

            Undo.IncrementCurrentGroup();
            Assert.AreEqual(0, fnCountEdge());

            var edgePresenterSin = VFXDataEdgePresenter.CreateInstance<VFXDataEdgePresenter>();
            edgePresenterSin.Init(cosPresenter.outputPorts[0], sinPresenter.inputPorts[0]);
            m_ViewPresenter.AddElement(edgePresenterSin);
            Assert.AreEqual(1, fnCountEdge());

            Undo.PerformUndo();
            Assert.AreEqual(0, fnCountEdge());
            Assert.NotNull(fnFindPresenter(typeof(VFXOperatorCosine)));
            Assert.NotNull(fnFindPresenter(typeof(VFXOperatorSine)));
        }

        [Test]
        public void UndoRedoOperatorLinkToBlock()
        {
            Func<VFXContextPresenter> fnFirstContextPresenter = delegate()
                {
                    return m_ViewPresenter.allChildren.OfType<VFXContextPresenter>().FirstOrDefault();
                };

            Func<Type, VFXSlotContainerPresenter> fnFindPresenter = delegate(Type type)
                {
                    var allPresenter = m_ViewPresenter.allChildren.OfType<VFXSlotContainerPresenter>();
                    return allPresenter.FirstOrDefault(o => type.IsInstanceOfType(o.slotContainer));
                };

            Func<VFXBlockPresenter> fnFirstBlockPresenter = delegate()
                {
                    return m_ViewPresenter.allChildren.OfType<VFXBlockPresenter>().FirstOrDefault();
                };

            Func<VFXDataEdgePresenter> fnFirstEdgePresenter = delegate()
                {
                    return m_ViewPresenter.allChildren.OfType<VFXDataEdgePresenter>().FirstOrDefault();
                };

            Undo.IncrementCurrentGroup();
            var cosDesc = VFXLibrary.GetOperators().FirstOrDefault(o => o.name == "Cosine");
            var contextUpdateDesc = VFXLibrary.GetContexts().FirstOrDefault(o => o.name.Contains("Update"));
            var blockAttributeDesc = VFXLibrary.GetBlocks().FirstOrDefault(o => o.modelType == typeof(Block.SetAttribute));

            var cos = m_ViewPresenter.AddVFXOperator(new Vector2(0, 0), cosDesc);
            var update = m_ViewPresenter.AddVFXContext(new Vector2(2, 2), contextUpdateDesc);
            var blockAttribute = blockAttributeDesc.CreateInstance();
            blockAttribute.SetSettingValue("attribute", "color");
            fnFirstContextPresenter().AddBlock(0, blockAttribute);

            var edgePresenter = VFXDataEdgePresenter.CreateInstance<VFXDataEdgePresenter>();
            edgePresenter.Init(fnFindPresenter(typeof(VFXOperatorCosine)).outputPorts[0], fnFirstBlockPresenter().inputPorts[0]);
            m_ViewPresenter.AddElement(edgePresenter);
            Undo.IncrementCurrentGroup();

            m_ViewPresenter.RemoveElement(fnFirstEdgePresenter());
            Assert.IsNull(fnFirstEdgePresenter());
            Undo.IncrementCurrentGroup();

            Undo.PerformUndo();
            Assert.IsNotNull(fnFirstEdgePresenter());

            Undo.PerformRedo();
            Assert.IsNull(fnFirstEdgePresenter());
        }

        [Test]
        public void UndoRedoOperatorLinkAdvanced()
        {
            Func<Type, VFXSlotContainerPresenter> fnFindPresenter = delegate(Type type)
                {
                    var allPresenter = m_ViewPresenter.allChildren.OfType<VFXSlotContainerPresenter>();
                    return allPresenter.FirstOrDefault(o => type.IsInstanceOfType(o.slotContainer));
                };

            Func<int> fnCountEdge = delegate()
                {
                    return m_ViewPresenter.allChildren.OfType<VFXDataEdgePresenter>().Count();
                };


            var absDesc = VFXLibrary.GetOperators().FirstOrDefault(o => o.name == "Absolute");
            var appendDesc = VFXLibrary.GetOperators().FirstOrDefault(o => o.name == "AppendVector");
            var crossDesc = VFXLibrary.GetOperators().FirstOrDefault(o => o.name == "Cross Product");
            var cosDesc = VFXLibrary.GetOperators().FirstOrDefault(o => o.name == "Cosine");
            var sinDesc = VFXLibrary.GetOperators().FirstOrDefault(o => o.name == "Sine");

            var abs = m_ViewPresenter.AddVFXOperator(new Vector2(0, 0), absDesc);
            var append = m_ViewPresenter.AddVFXOperator(new Vector2(1, 1), appendDesc);
            var cross = m_ViewPresenter.AddVFXOperator(new Vector2(2, 2), crossDesc);
            var cos = m_ViewPresenter.AddVFXOperator(new Vector2(3, 3), cosDesc);
            var sin = m_ViewPresenter.AddVFXOperator(new Vector2(4, 4), sinDesc);

            var absPresenter = fnFindPresenter(typeof(VFXOperatorAbsolute));
            var appendPresenter = fnFindPresenter(typeof(VFXOperatorAppendVector));
            var crossPresenter = fnFindPresenter(typeof(VFXOperatorCrossProduct));

            for (int i = 0; i < 3; ++i)
            {
                var edgePresenter = VFXDataEdgePresenter.CreateInstance<VFXDataEdgePresenter>();
                edgePresenter.Init(absPresenter.outputPorts[0], appendPresenter.inputPorts[i]);
                m_ViewPresenter.AddElement(edgePresenter);
            }

            var edgePresenterCross = VFXDataEdgePresenter.CreateInstance<VFXDataEdgePresenter>();
            edgePresenterCross.Init(appendPresenter.outputPorts[0], crossPresenter.inputPorts[0]);
            m_ViewPresenter.AddElement(edgePresenterCross);

            Undo.IncrementCurrentGroup();
            Assert.AreEqual(4, fnCountEdge());
            Assert.IsInstanceOf(typeof(VFXSlotFloat3), (appendPresenter.outputPorts[0] as VFXDataAnchorPresenter).model);

            //Find last edge in append node
            var referenceAnchor = appendPresenter.inputPorts[2];
            var edgeToDelete = m_ViewPresenter.allChildren
                .OfType<VFXDataEdgePresenter>()
                .Cast<VFXDataEdgePresenter>()
                .FirstOrDefault(e =>
                {
                    return e.input == referenceAnchor;
                });
            Assert.NotNull(edgeToDelete);

            m_ViewPresenter.RemoveElement(edgeToDelete);
            Assert.AreEqual(2, fnCountEdge()); //cross should be implicitly disconnected ...
            Assert.IsInstanceOf(typeof(VFXSlotFloat2), (appendPresenter.outputPorts[0] as VFXDataAnchorPresenter).model);

            Undo.PerformUndo();
            Assert.AreEqual(4, fnCountEdge()); //... and restored !
            Assert.IsInstanceOf(typeof(VFXSlotFloat3), (fnFindPresenter(typeof(VFXOperatorAppendVector)).outputPorts[0] as VFXDataAnchorPresenter).model);
            Undo.PerformRedo();
            Assert.AreEqual(2, fnCountEdge());
            Assert.IsInstanceOf(typeof(VFXSlotFloat2), (fnFindPresenter(typeof(VFXOperatorAppendVector)).outputPorts[0] as VFXDataAnchorPresenter).model);

            //Improve test connecting cos & sin => then try delete append
            Undo.PerformUndo();
            Undo.IncrementCurrentGroup();
            Assert.AreEqual(4, fnCountEdge());
            Assert.IsInstanceOf(typeof(VFXSlotFloat3), (fnFindPresenter(typeof(VFXOperatorAppendVector)).outputPorts[0] as VFXDataAnchorPresenter).model);

            var edgePresenterCos = VFXDataEdgePresenter.CreateInstance<VFXDataEdgePresenter>();
            edgePresenterCos.Init(fnFindPresenter(typeof(VFXOperatorAppendVector)).outputPorts[0], fnFindPresenter(typeof(VFXOperatorCosine)).inputPorts[0]);
            m_ViewPresenter.AddElement(edgePresenterCos);
            Assert.AreEqual(5, fnCountEdge());

            var edgePresenterSin = VFXDataEdgePresenter.CreateInstance<VFXDataEdgePresenter>();
            edgePresenterSin.Init(fnFindPresenter(typeof(VFXOperatorAppendVector)).outputPorts[0], fnFindPresenter(typeof(VFXOperatorSine)).inputPorts[0]);
            m_ViewPresenter.AddElement(edgePresenterSin);
            Assert.AreEqual(6, fnCountEdge());

            m_ViewPresenter.RemoveElement(fnFindPresenter(typeof(VFXOperatorAppendVector)));
            Assert.AreEqual(0, fnCountEdge());
        }

        [Test]
        public void UndoRedoOperatorSettings()
        {
            Func<VFXOperatorPresenter> fnFirstOperatorPresenter = delegate()
                {
                    return m_ViewPresenter.allChildren.OfType<VFXOperatorPresenter>().FirstOrDefault();
                };

            Action<VFXOperatorComponentMask, string> fnSetSetting = delegate(VFXOperatorComponentMask target, string mask)
                {
                    target.x = target.y = target.z = target.w = VFXOperatorComponentMask.Component.None;
                    for (int i = 0; i < mask.Length; ++i)
                    {
                        var current = (VFXOperatorComponentMask.Component)Enum.Parse(typeof(VFXOperatorComponentMask.Component),  mask[i].ToString().ToUpper());
                        if (i == 0)
                        {
                            target.x = current;
                        }
                        else if (i == 1)
                        {
                            target.y = current;
                        }
                        else if (i == 2)
                        {
                            target.z = current;
                        }
                        else if (i == 3)
                        {
                            target.w = current;
                        }
                    }
                    target.Invalidate(VFXModel.InvalidationCause.kSettingChanged);
                };

            Func<VFXOperatorComponentMask, string> fnGetSetting = delegate(VFXOperatorComponentMask target)
                {
                    var value = "";
                    if (target.x != VFXOperatorComponentMask.Component.None)
                        value += target.x.ToString().ToLower();
                    if (target.y != VFXOperatorComponentMask.Component.None)
                        value += target.y.ToString().ToLower();
                    if (target.z != VFXOperatorComponentMask.Component.None)
                        value += target.z.ToString().ToLower();
                    if (target.w != VFXOperatorComponentMask.Component.None)
                        value += target.w.ToString().ToLower();
                    return value;
                };

            var componentMaskDesc = VFXLibrary.GetOperators().FirstOrDefault(o => o.name == "ComponentMask");
            var componentMask = m_ViewPresenter.AddVFXOperator(new Vector2(0, 0), componentMaskDesc);

            var maskList = new string[] { "xy", "yww", "xw", "z" };
            for (int i = 0; i < maskList.Length; ++i)
            {
                var componentMaskPresenter = fnFirstOperatorPresenter();
                Undo.IncrementCurrentGroup();
                fnSetSetting(componentMaskPresenter.model as VFXOperatorComponentMask, maskList[i]);
                Assert.AreEqual(maskList[i], fnGetSetting(componentMaskPresenter.model as VFXOperatorComponentMask));
            }

            for (int i = maskList.Length - 1; i > 0; --i)
            {
                Undo.PerformUndo();
                var componentMaskPresenter = fnFirstOperatorPresenter();
                Assert.AreEqual(maskList[i - 1], fnGetSetting(componentMaskPresenter.model as VFXOperatorComponentMask));
            }

            for (int i = 0; i < maskList.Length - 1; ++i)
            {
                Undo.PerformRedo();
                var componentMaskPresenter = fnFirstOperatorPresenter();
                Assert.AreEqual(maskList[i + 1], fnGetSetting(componentMaskPresenter.model as VFXOperatorComponentMask));
            }
        }

        [Test]
        public void UndoRedoAddBlockContext()
        {
            var contextUpdateDesc = VFXLibrary.GetContexts().FirstOrDefault(o => o.name.Contains("Update"));
            var blockDesc = VFXLibrary.GetBlocks().FirstOrDefault(o => o.modelType == typeof(AllType));

            var contextUpdate = m_ViewPresenter.AddVFXContext(Vector2.one, contextUpdateDesc);
            Func<VFXContextPresenter> fnContextPresenter = delegate()
                {
                    var allContextPresenter = m_ViewPresenter.allChildren.OfType<VFXContextPresenter>().ToArray();
                    return allContextPresenter.FirstOrDefault() as VFXContextPresenter;
                };
            Assert.IsNotNull(fnContextPresenter());
            //Creation
            Undo.IncrementCurrentGroup();
            fnContextPresenter().AddBlock(0, blockDesc.CreateInstance());
            Assert.AreEqual(1, fnContextPresenter().context.children.Count());
            Undo.PerformUndo();
            Assert.AreEqual(0, fnContextPresenter().context.children.Count());

            //Deletion
            var block = blockDesc.CreateInstance();
            fnContextPresenter().AddBlock(0, block);
            Assert.AreEqual(1, fnContextPresenter().context.children.Count());
            Undo.IncrementCurrentGroup();
            fnContextPresenter().RemoveBlock(block);
            Assert.AreEqual(0, fnContextPresenter().context.children.Count());

            Undo.PerformUndo();
            Assert.IsNotNull(fnContextPresenter());
            Assert.AreEqual(1, fnContextPresenter().context.children.Count());
            Assert.IsInstanceOf(typeof(AllType), fnContextPresenter().context.children.First());
        }

        [Test]
        public void UndoRedoContext()
        {
            Func<VFXContextPresenter> fnFirstContextPresenter = delegate()
                {
                    return m_ViewPresenter.allChildren.OfType<VFXContextPresenter>().FirstOrDefault() as VFXContextPresenter;
                };

            var contextDesc = VFXLibrary.GetContexts().FirstOrDefault();
            Undo.IncrementCurrentGroup();
            m_ViewPresenter.AddVFXContext(Vector2.zero, contextDesc);

            Assert.NotNull(fnFirstContextPresenter());
            Undo.PerformUndo();
            Assert.Null(fnFirstContextPresenter(), "Fail Undo Create");

            Undo.IncrementCurrentGroup();
            m_ViewPresenter.AddVFXContext(Vector2.zero, contextDesc);
            Assert.NotNull(fnFirstContextPresenter());

            Undo.IncrementCurrentGroup();
            m_ViewPresenter.RemoveElement(fnFirstContextPresenter());
            Assert.Null(fnFirstContextPresenter());

            Undo.PerformUndo();
            Assert.NotNull(fnFirstContextPresenter(), "Fail Undo Delete");
        }

        [Test]
        public void UndoRedoContextLinkMultiSlot()
        {
            Func<VFXContextPresenter[]> fnContextPresenter = delegate()
                {
                    return m_ViewPresenter.allChildren.OfType<VFXContextPresenter>().Cast<VFXContextPresenter>().ToArray();
                };

            Func<VFXContextPresenter> fnSpawner = delegate()
                {
                    var presenter = fnContextPresenter();
                    return presenter.FirstOrDefault(o => o.model.name.Contains("Spawner"));
                };

            Func<string, VFXContextPresenter> fnEvent = delegate(string name)
                {
                    var presenter = fnContextPresenter();
                    var allEvent = presenter.Where(o => o.model.name.Contains("Event"));
                    return allEvent.FirstOrDefault(o => (o.model as VFXBasicEvent).eventName == name) as VFXContextPresenter;
                };

            Func<VFXContextPresenter> fnStart = delegate()
                {
                    return fnEvent("Start");
                };

            Func<VFXContextPresenter> fnStop = delegate()
                {
                    return fnEvent("Stop");
                };

            Func<int> fnFlowEdgeCount = delegate()
                {
                    return m_ViewPresenter.allChildren.OfType<VFXFlowEdgePresenter>().Count();
                };

            var contextSpawner = VFXLibrary.GetContexts().FirstOrDefault(o => o.name.Contains("Spawner"));
            var contextEvent = VFXLibrary.GetContexts().FirstOrDefault(o => o.name.Contains("Event"));

            m_ViewPresenter.AddVFXContext(new Vector2(1, 1), contextSpawner);
            var eventStartPresenter = m_ViewPresenter.AddVFXContext(new Vector2(2, 2), contextEvent) as VFXBasicEvent;
            var eventStopPresenter = m_ViewPresenter.AddVFXContext(new Vector2(3, 3), contextEvent) as VFXBasicEvent;
            eventStartPresenter.SetSettingValue("eventName", "Start");
            eventStopPresenter.SetSettingValue("eventName", "Stop");

            //Creation
            var flowEdge = VFXFlowEdgePresenter.CreateInstance<VFXFlowEdgePresenter>();
            flowEdge.Init(fnSpawner().flowInputAnchors.ElementAt(0), fnStart().flowOutputAnchors.FirstOrDefault());

            Undo.IncrementCurrentGroup();
            m_ViewPresenter.AddElement(flowEdge);
            Assert.AreEqual(1, fnFlowEdgeCount());

            flowEdge = VFXFlowEdgePresenter.CreateInstance<VFXFlowEdgePresenter>();
            flowEdge.Init(fnSpawner().flowInputAnchors.ElementAt(1), fnStop().flowOutputAnchors.FirstOrDefault());

            Undo.IncrementCurrentGroup();
            m_ViewPresenter.AddElement(flowEdge);
            Assert.AreEqual(2, fnFlowEdgeCount());

            //Test a single deletion
            var allFlowEdge = m_ViewPresenter.allChildren.OfType<VFXFlowEdgePresenter>().ToArray();

            // Integrity test...
            var inputSlotIndex = allFlowEdge.Select(o => (o.input as VFXFlowAnchorPresenter).slotIndex).OrderBy(o => o).ToArray();
            var outputSlotIndex = allFlowEdge.Select(o => (o.output as VFXFlowAnchorPresenter).slotIndex).OrderBy(o => o).ToArray();

            Assert.AreEqual(inputSlotIndex[0], 0);
            Assert.AreEqual(inputSlotIndex[1], 1);
            Assert.AreEqual(outputSlotIndex[0], 0);
            Assert.AreEqual(outputSlotIndex[1], 0);

            var edge = allFlowEdge.First(o => o.input == fnSpawner().flowInputAnchors.ElementAt(1) && o.output == fnStop().flowOutputAnchors.FirstOrDefault());

            Undo.IncrementCurrentGroup();
            m_ViewPresenter.RemoveElement(edge);
            Assert.AreEqual(1, fnFlowEdgeCount());

            Undo.PerformUndo();
            Assert.AreEqual(2, fnFlowEdgeCount());

            Undo.PerformRedo();
            Assert.AreEqual(1, fnFlowEdgeCount());

            Undo.PerformUndo();
            Assert.AreEqual(2, fnFlowEdgeCount());

            //Global Deletion
            Undo.PerformUndo();
            Assert.AreEqual(1, fnFlowEdgeCount());

            Undo.PerformUndo();
            Assert.AreEqual(0, fnFlowEdgeCount());
        }

        [Test]
        public void UndoRedoContextLink()
        {
            Func<VFXContextPresenter[]> fnContextPresenter = delegate()
                {
                    return m_ViewPresenter.allChildren.OfType<VFXContextPresenter>().Cast<VFXContextPresenter>().ToArray();
                };

            Func<VFXContextPresenter> fnInitializePresenter = delegate()
                {
                    var presenter = fnContextPresenter();
                    return presenter.FirstOrDefault(o => o.model.name.Contains("Init"));
                };

            Func<VFXContextPresenter> fnUpdatePresenter = delegate()
                {
                    var presenter = fnContextPresenter();
                    return presenter.FirstOrDefault(o => o.model.name.Contains("Update"));
                };

            Func<int> fnFlowEdgeCount = delegate()
                {
                    return m_ViewPresenter.allChildren.OfType<VFXFlowEdgePresenter>().Count();
                };

            var contextInitializeDesc = VFXLibrary.GetContexts().FirstOrDefault(o => o.name.Contains("Init"));
            var contextUpdateDesc = VFXLibrary.GetContexts().FirstOrDefault(o => o.name.Contains("Update"));

            var contextInitialize = m_ViewPresenter.AddVFXContext(new Vector2(1, 1), contextInitializeDesc);
            var contextUpdate = m_ViewPresenter.AddVFXContext(new Vector2(2, 2), contextUpdateDesc);

            //Creation
            var flowEdge = VFXFlowEdgePresenter.CreateInstance<VFXFlowEdgePresenter>();
            flowEdge.Init(fnUpdatePresenter().flowInputAnchors.FirstOrDefault(), fnInitializePresenter().flowOutputAnchors.FirstOrDefault());

            Undo.IncrementCurrentGroup();
            m_ViewPresenter.AddElement(flowEdge);
            Assert.AreEqual(1, fnFlowEdgeCount());

            Undo.PerformUndo();
            Assert.AreEqual(0, fnFlowEdgeCount(), "Fail undo Create");

            //Deletion
            flowEdge = VFXFlowEdgePresenter.CreateInstance<VFXFlowEdgePresenter>();
            flowEdge.Init(fnUpdatePresenter().flowInputAnchors.FirstOrDefault(), fnInitializePresenter().flowOutputAnchors.FirstOrDefault());
            m_ViewPresenter.AddElement(flowEdge);
            Assert.AreEqual(1, fnFlowEdgeCount());

            Undo.IncrementCurrentGroup();
            m_ViewPresenter.RemoveElement(m_ViewPresenter.allChildren.OfType<VFXFlowEdgePresenter>().FirstOrDefault());
            Assert.AreEqual(0, fnFlowEdgeCount());

            Undo.PerformUndo();
            Assert.AreEqual(1, fnFlowEdgeCount(), "Fail undo Delete");
        }

        [Test]
        public void DeleteSubSlotWithLink()
        {
            var crossProductDesc = VFXLibrary.GetOperators().FirstOrDefault(o => o.name.Contains("Cross"));
            var sinDesc = VFXLibrary.GetOperators().FirstOrDefault(o => o.name.Contains("Sin"));
            var cosDesc = VFXLibrary.GetOperators().FirstOrDefault(o => o.name.Contains("Cos"));

            var crossProduct = m_ViewPresenter.AddVFXOperator(new Vector2(0, 0), crossProductDesc);
            var sin = m_ViewPresenter.AddVFXOperator(new Vector2(8, 8), sinDesc);
            var cos = m_ViewPresenter.AddVFXOperator(new Vector2(-8, 8), cosDesc);

            crossProduct.outputSlots[0].children.ElementAt(1).Link(sin.inputSlots[0]);
            crossProduct.outputSlots[0].children.ElementAt(1).Link(cos.inputSlots[0]);

            var crossPresenter = m_ViewPresenter.allChildren.OfType<VFXOperatorPresenter>().First(o => o.model.name.Contains("Cross"));
            m_ViewPresenter.RemoveElement(crossPresenter);

            Assert.IsFalse(cos.inputSlots[0].HasLink(true));
            Assert.IsFalse(sin.inputSlots[0].HasLink(true));
        }
    }
}
