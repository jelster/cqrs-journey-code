﻿// ==============================================================================================================
// Microsoft patterns & practices
// CQRS Journey project
// ==============================================================================================================
// ©2012 Microsoft. All rights reserved. Certain content used with permission from contributors
// http://cqrsjourney.github.com/contributors/members
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance 
// with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0
// Unless required by applicable law or agreed to in writing, software distributed under the License is 
// distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. 
// See the License for the specific language governing permissions and limitations under the License.
// ==============================================================================================================

namespace Infrastructure.Serialization
{
    using System.IO;
    using Newtonsoft.Json;

    public class JsonSerializerAdapter : ISerializer
    {
        private JsonSerializer serializer;

        public JsonSerializerAdapter(JsonSerializer serializer)
        {
            this.serializer = serializer;
        }

        public void Serialize(Stream stream, object graph)
        {
            var writer = new JsonTextWriter(new StreamWriter(stream));
#if DEBUG
            writer.Formatting = Formatting.Indented;
#endif

            this.serializer.Serialize(writer, graph);

            // We don't close the stream as it's owned by the message.
            writer.Flush();
        }

        public object Deserialize(Stream stream)
        {
            var reader = new JsonTextReader(new StreamReader(stream));

            return this.serializer.Deserialize(reader);
        }
    }
}
