﻿// <copyright file="ResolutionExamples.cs" company="Basho Technologies, Inc.">
// Copyright 2015 - Basho Technologies, Inc.
//
// This file is provided to you under the Apache License,
// Version 2.0 (the "License"); you may not use this file
// except in compliance with the License.  You may obtain
// a copy of the License at
//
//   http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing,
// software distributed under the License is distributed on an
// "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
// KIND, either express or implied.  See the License for the
// specific language governing permissions and limitations
// under the License.
// </copyright>

namespace RiakClientExamples.Dev.Using.ConflictResolution
{
    using System;
    using System.Diagnostics;
    using System.Linq;
    using System.Text;
    using NUnit.Framework;
    using RiakClient;
    using RiakClient.Models;

    /*
     * http://docs.basho.com/riak/latest/dev/using/conflict-resolution/
     * http://docs.basho.com/riak/latest/dev/using/conflict-resolution/csharp
     */
    public sealed class ResolutionExamples : ExampleBase
    {
        [Test]
        public void StoringTwoValuesToSameKeyShowingSiblings()
        {
            id = PutNickolodeonCharacters();

            var getResult = client.Get(id);
            RiakObject obj = getResult.Value;
            Assert.AreEqual(2, obj.Siblings.Count);
            Console.WriteLine("Sibling count: {0}", obj.Siblings.Count);
            foreach (var sibling in obj.Siblings)
            {
                Console.WriteLine("    VTag: {0}", sibling.VTag);
            }
        }

        [Test]
        public void ResolvingSiblingsWithUpdate()
        {
            id = PutNickolodeonCharacters();

            // First, fetch the object
            var getResult = client.Get(id);

            // Then, modify the object's value
            RiakObject obj = getResult.Value;
            obj.SetObject<string>("Stimpy", RiakConstants.ContentTypes.TextPlain);

            // Then, store the object which has vector clock attached
            var putRslt = client.Put(obj);
            CheckResult(putRslt);

            obj = putRslt.Value;
            // Voila, no more siblings!
            Debug.Assert(obj.Siblings.Count == 0);
            Assert.AreEqual(0, obj.Siblings.Count);
        }

        [Test]
        public void ResolvingSiblingsWithUpdateUsingFirstSibling()
        {
            id = PutNickolodeonCharacters();

            // First, fetch the object
            var getResult = client.Get(id);

            // Then, pick the first sibling
            RiakObject chosenSibling = getResult.Value.Siblings.First();

            // Then, store the chosen object
            var putRslt = client.Put(chosenSibling);
            CheckResult(putRslt);

            RiakObject updatedObject = putRslt.Value;
            // Voila, no more siblings!
            Debug.Assert(updatedObject.Siblings.Count == 0);
            Assert.AreEqual(0, updatedObject.Siblings.Count);
            Assert.AreEqual("Ren", Encoding.UTF8.GetString(updatedObject.Value));
        }

        private RiakObjectId PutNickolodeonCharacters()
        {
            var id = new RiakObjectId("siblings_allowed", "nickolodeon", "best_character");

            var renObj = new RiakObject(id, "Ren", RiakConstants.ContentTypes.TextPlain);
            var stimpyObj = new RiakObject(id, "Stimpy", RiakConstants.ContentTypes.TextPlain);

            var renResult = client.Put(renObj);
            CheckResult(renResult);

            var stimpyResult = client.Put(stimpyObj);
            CheckResult(stimpyResult);

            return id;
        }
    }
}
