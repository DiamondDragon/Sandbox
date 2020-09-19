using System;
using GraphQL;
using GraphQL.Types;
using StarWars.Types;

namespace StarWars
{
    /// <example>
    /// This is an example JSON request for a mutation
    /// {
    ///   "query": "mutation ($human:HumanInput!){ createHuman(human: $human) { id name } }",
    ///   "variables": {
    ///     "human": {
    ///       "name": "Boba Fett"
    ///     }
    ///   }
    /// }
    /// </example>
    public class StarWarsMutation : ObjectGraphType
    {
        public StarWarsMutation(StarWarsData data)
        {
            Name = "Mutation";

            Field<HumanType>(
                "createHuman",
                arguments: new QueryArguments(
                    new QueryArgument<HumanInputType> {Name = "human111"}
                ),
                resolve: context =>
                {
                    var human = context.GetArgument<Human>("human111");

                    human = human ?? new Human
                    {
                        Id = Guid.NewGuid().ToString(),
                        Name = "User" + Guid.NewGuid()
                    };

                    return data.AddHuman(human);
                });
        }
    }
}
