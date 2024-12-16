using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using sReportsV2.Common.Constants;
using sReportsV2.Domain.Entities.FormInstance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.Domain.FieldFilters
{
    internal static class CommonFilters
    {
        internal static FilterDefinition<FieldInstance> FieldThesaurusIdFilter(int fieldThesaurusId)
        {
            return Builders<FieldInstance>.Filter.Eq(x => x.ThesaurusId, fieldThesaurusId);
        }
    }

    public static class EqualityFiltersBuilder 
    {
        public static FilterDefinition<FormInstance> Equal(string value, int fieldThesaurusId)
        {
            return Builders<FormInstance>.Filter.ElemMatch(
                x => x.FieldInstances,
                CommonFilters.FieldThesaurusIdFilter(fieldThesaurusId)
                & Builders<FieldInstance>.Filter.AnyEq(x => x.FieldInstanceValues.Select(y => y.ValueLabel), value)
                );
        }

        public static FilterDefinition<FormInstance> NotEqual(string value, int fieldThesaurusId)
        {
            return Builders<FormInstance>.Filter.ElemMatch(
                x => x.FieldInstances,
                CommonFilters.FieldThesaurusIdFilter(fieldThesaurusId)
                & Builders<FieldInstance>.Filter.AnyNe(x => x.FieldInstanceValues.Select(y => y.ValueLabel), value)
                );
        }
    }

    public static class TextualFiltersBuilder 
    {
        public static FilterDefinition<FormInstance> Contains(string value, int fieldThesaurusId)
        {
            return Builders<FormInstance>.Filter.ElemMatch(
                x => x.FieldInstances,
                CommonFilters.FieldThesaurusIdFilter(fieldThesaurusId) 
                & Builders<FieldInstance>.Filter.Regex(x => x.FieldInstanceValues.Select(y => y.ValueLabel), new BsonRegularExpression(value, "i"))  // i: case insensitive option 
                );
        }
    }

    public static class ComparisonFiltersBuilder
    {
        public static FilterDefinition<FormInstance> LessThan(string value, int fieldThesaurusId)
        {
            return Builders<FormInstance>.Filter.ElemMatch(
                x => x.FieldInstances,
                CommonFilters.FieldThesaurusIdFilter(fieldThesaurusId)
                & Builders<FieldInstance>.Filter.AnyLt(x => x.FieldInstanceValues.Select(y => y.ValueLabel), value)
                );
        }

        public static FilterDefinition<FormInstance> LessThanOrEqual(string value, int fieldThesaurusId)
        {
            return Builders<FormInstance>.Filter.ElemMatch(
                x => x.FieldInstances,
                CommonFilters.FieldThesaurusIdFilter(fieldThesaurusId)
                & Builders<FieldInstance>.Filter.AnyLt(x => x.FieldInstanceValues.Select(y => y.ValueLabel), value)
                );
        }

        public static FilterDefinition<FormInstance> GreaterThanOrEqual(string value, int fieldThesaurusId)
        {
            return Builders<FormInstance>.Filter.ElemMatch(
                x => x.FieldInstances,
                CommonFilters.FieldThesaurusIdFilter(fieldThesaurusId)
                & Builders<FieldInstance>.Filter.AnyGt(x => x.FieldInstanceValues.Select(y => y.ValueLabel), value)
                );
        }

        public static FilterDefinition<FormInstance> GreaterThan(string value, int fieldThesaurusId)
        {
            return Builders<FormInstance>.Filter.ElemMatch(
                x => x.FieldInstances,
                CommonFilters.FieldThesaurusIdFilter(fieldThesaurusId)
                & Builders<FieldInstance>.Filter.AnyGte(x => x.FieldInstanceValues.Select(y => y.ValueLabel), value)
                );
        }
    }
}
