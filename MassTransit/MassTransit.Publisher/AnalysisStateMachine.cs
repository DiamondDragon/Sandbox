using System;
using Automatonymous;

namespace MassTransit.Publisher
{
    public class AnalysisStateMachineInstance : SagaStateMachineInstance
    {
        #region ISaga

        public Guid CorrelationId { get; set; }

        #endregion

        public string CurrentState { get; set; }

        public string ProjectName { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string Location { get; set; }
    }

    public class AnalysisStateMachine : MassTransitStateMachine<AnalysisStateMachineInstance>
    {
        public AnalysisStateMachine()
        {
            InstanceState(x => x.CurrentState);

            Event(() => StartAnalysis, x => x.CorrelateBy(state => state.ProjectName, context => context.Message.ProjectName).SelectId(context => Guid.NewGuid()));
            Event(() => SourceCodeLocationResolved, x => x.CorrelateById(context => context.Message.AnalysisId));
            Event(() => SourceCodeDownloaded, x => x.CorrelateById(context => context.Message.AnalysisId));

            Initially(
                When(StartAnalysis)
                    .Then(context =>
                    {
                        context.Instance.CreatedAt = DateTime.Now;
                        context.Instance.UpdatedAt = DateTime.Now;
                        context.Instance.ProjectName = context.Data.ProjectName;
                    })
                    .ThenAsync(context => Console.Out.WriteLineAsync("Project name" + context.Data.ProjectName))
                    .TransitionTo(WaitingForSourceCodeLocationResolution)
            );

            During(WaitingForSourceCodeLocationResolution,
                When(SourceCodeLocationResolved)
                    .Then(context =>
                    {
                        context.Instance.Location = context.Data.Branch;
                    })
                    .ThenAsync(context => Console.Out.WriteLineAsync("Source code resolver"))
                    .TransitionTo(WaitingSourceCodeDownload)
            );

            During(WaitingSourceCodeDownload,
                When(SourceCodeLocationResolved)
                    .Then(context =>
                    {
                        context.Instance.Location = context.Data.Branch;
                    })
                    .ThenAsync(context => Console.Out.WriteLineAsync("Source code download"))
                    .Finalize()
            );
        }

        #region Events

        public Event<StartAnalysisCommand> StartAnalysis { get; private set; }
        public Event<SourceCodeLocationResolveCompleted> SourceCodeLocationResolved { get; private set; }
        public Event<SourceCodeDownloaded> SourceCodeDownloaded { get; private set; }

        #endregion

        //#region Schedule

        //public Schedule<ShoppingCart, CartExpired> CartExpired { get; private set; }

        //#endregion


        #region States

        public State WaitingForSourceCodeLocationResolution { get; set; }
        public State WaitingSourceCodeDownload { get; set; }

        #endregion


    }

    public class StartAnalysisCommand
    {
        public string ProjectName { get; set; }

    }

    public class SourceCodeLocationResolveCompleted
    {
        public string RepositoryName { get; set; }
        public string Branch { get; set; }
        public Guid AnalysisId { get; set; }
    }

    public class SourceCodeDownloaded
    {
        public string FolderPath { get; set; }
        public Guid AnalysisId { get; set; }
    }
}