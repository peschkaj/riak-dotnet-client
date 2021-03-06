// <copyright file="RiakBucketSearchInput.cs" company="Basho Technologies, Inc.">
// Copyright 2011 - OJ Reeves & Jeremiah Peschka
// Copyright 2014 - Basho Technologies, Inc.
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

namespace RiakClient.Models.MapReduce.Inputs
{
    using System;
    using Newtonsoft.Json;
    using Search;

    /// <summary>
    /// Represents a legacy search based mapreduce input.
    /// </summary>
    [Obsolete("Using Legacy Search as input for MapReduce is deprecated. Please move to Riak 2.0 Search, and use the RiakSearchInput class instead.")]
    public class RiakBucketSearchInput : RiakPhaseInput
    {
        private readonly string bucket;
        private readonly string query;
        private string filter;

        /// <summary>
        /// Initializes a new instance of the <see cref="RiakBucketSearchInput"/> class.
        /// </summary>
        /// <param name="query">The <see cref="RiakFluentSearch"/> to run, whose results will be used as inputs for the mapreduce job. </param>
        public RiakBucketSearchInput(RiakFluentSearch query)
            : this(query.Index, query.ToString())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RiakBucketSearchInput"/> class.
        /// </summary>
        /// <param name="bucket">The bucket to run the legacy search <paramref name="query"/> against.</param>
        /// <param name="query">The query to run, whose results will be used as inputs for the mapreduce job.</param>
        public RiakBucketSearchInput(string bucket, string query)
        {
            this.bucket = bucket;
            this.query = query;
        }

        /// <summary>
        /// Filter the main query with a secondary <see cref="RiakFluentSearch"/> query.
        /// </summary>
        /// <param name="filter">The secondary filter query.</param>
        /// <returns>A reference to this updated instance, for fluent chaining.</returns>
        public RiakBucketSearchInput Filter(RiakFluentSearch filter)
        {
            return Filter(filter.ToString());
        }

        /// <summary>
        /// Filter the main query with a secondary query.
        /// </summary>
        /// <param name="filter">The secondary filter query.</param>
        /// <returns>A reference to this updated instance, for fluent chaining.</returns>
        public RiakBucketSearchInput Filter(string filter)
        {
            this.filter = filter;
            return this;
        }

        /// <inheritdoc/>
        public override JsonWriter WriteJson(JsonWriter writer)
        {
            writer.WritePropertyName("inputs");
            writer.WriteStartObject();

            writer.WritePropertyName("bucket");
            writer.WriteValue(bucket);

            writer.WritePropertyName("query");
            writer.WriteValue(query);

            if (!string.IsNullOrEmpty(filter))
            {
                writer.WritePropertyName("filter");
                writer.WriteValue(filter);
            }

            writer.WriteEndObject();

            return writer;
        }
    }
}
