using System;

namespace OpenDataEngine.Model.Relation
{
    // public class HasOne<TModel> : Query.Query<TModel>, IRelation<TModel>
    // {
    //     private readonly TModel _model;
    //
    //     private HasOne(TModel model)
    //     {
    //         _model = model;
    //     }
    //
    //     public static implicit operator TModel(HasOne<TModel> self) => default!;
    //     public static implicit operator HasOne<TModel>(TModel model) => new HasOne<TModel>(model);
    // }

    [AttributeUsage(AttributeTargets.Property)]
    public class HasOne : System.Attribute
    {
        public HasOne()
        {
        }
    }
}