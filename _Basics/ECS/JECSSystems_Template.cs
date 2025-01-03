#if JDOTS_SUPPORT
using Unity.Burst;
using Unity.Entities;

namespace JReact
{
    //fixed update
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    public partial struct JFixedUpdateSystem : ISystem
    {
        [BurstCompile] public void OnCreate(ref SystemState state) {}

        [BurstCompile] public void OnUpdate(ref SystemState state) {}

        [BurstCompile] public void OnDestroy(ref SystemState state) {}
    }
    
    //late update
    [UpdateInGroup(typeof(PresentationSystemGroup))]
    public partial struct JLateUpdateSystem : ISystem
    {
        [BurstCompile] public void OnCreate(ref SystemState state) {}

        [BurstCompile] public void OnUpdate(ref SystemState state) {}

        [BurstCompile] public void OnDestroy(ref SystemState state) {}
    }
    
    //normal Update(), the updated in group is optional
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial struct JUpdateSystem : ISystem
    {
        [BurstCompile] public void OnCreate(ref SystemState state) {}

        [BurstCompile] public void OnUpdate(ref SystemState state) {}

        [BurstCompile] public void OnDestroy(ref SystemState state) {}
    }
}
#endif
