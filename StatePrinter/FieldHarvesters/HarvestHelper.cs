﻿// Copyright 2014 Kasper B. Graversen
// 
// Licensed to the Apache Software Foundation (ASF) under one
// or more contributor license agreements.  See the NOTICE file
// distributed with this work for additional information
// regarding copyright ownership.  The ASF licenses this file
// to you under the Apache License, Version 2.0 (the
// "License"); you may not use this file except in compliance
// with the License.  You may obtain a copy of the License at
// 
//   http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing,
// software distributed under the License is distributed on an
// "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
// KIND, either express or implied.  See the License for the
// specific language governing permissions and limitations
// under the License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using StatePrinter.Configurations;

namespace StatePrinter.FieldHarvesters
{
  /// <summary>
  /// Reusable helper methods when implementing <see cref="IFieldHarvester"/>
  /// </summary>
  public class HarvestHelper
  {
    internal const string BackingFieldSuffix = ">k__BackingField";

    /// <summary>
    ///   We ignore all properties as they, in the end, will only point to some computated state or other fields.
    ///   Hence they do not provide information about the actual state of the object.
    /// </summary>
    public IEnumerable<FieldInfo> GetFields(Type type)
    {
      const BindingFlags flags = BindingFlags.Public
                            | BindingFlags.NonPublic
                            | BindingFlags.Instance
                            | BindingFlags.DeclaredOnly;

      return GetFields(type, flags);
    }

    /// <summary>
    ///   We ignore all properties as they, in the end, will only point to some computated state or other fields.
    ///   Hence they do not provide information about the actual state of the object.
    /// </summary>
    public IEnumerable<FieldInfo> GetFields(Type type, BindingFlags flags)
    {
      if (type == null)
        return Enumerable.Empty<FieldInfo>();

      if (!IsHarvestable(type))
        return Enumerable.Empty<FieldInfo>();

      return GetFields(type.BaseType, flags).Concat(type.GetFields(flags));
    }

    /// <summary>
    /// Tell if the type makes any sense to dump
    /// </summary>
    public bool IsHarvestable(Type type)
    {
      var typename = type.ToString();
      if (typename.StartsWith("System.Reflection")
          || typename.StartsWith("System.Runtime")
          || typename.StartsWith("System.SignatureStruct")
          || typename.StartsWith("System.Func"))
        return false;
      return true;
    }

    public string SanitizeFieldName(string fieldName)
    {
      return fieldName.StartsWith("<")
        ? fieldName.Substring(1).Replace(BackingFieldSuffix, "")
        : fieldName;
    }
  }
}