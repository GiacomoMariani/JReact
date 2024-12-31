using Unity.Burst;
using Unity.Jobs;

namespace JReact
{
    [BurstCompile( /*CompileSynchronously = true,*/ OptimizeFor = OptimizeFor.Performance)]
    public struct J_JobForTemplate : IJobFor
    {
        public void Execute(int index) {}

        //parameters here
        public static void Run(JobHandle inputDeps) { Schedule(inputDeps).Complete(); }

        public static JobHandle Schedule(JobHandle inputDeps)
        {
            //add data
            var job = new J_JobForTemplate() {};

            var amount    = 10;
            var jobHandle = job.ScheduleParallel(amount, 8, inputDeps);
            return jobHandle;
        }
    }
}
