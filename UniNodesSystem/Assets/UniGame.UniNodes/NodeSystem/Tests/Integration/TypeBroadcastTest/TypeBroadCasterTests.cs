namespace UniGame.UniNodes.NodeSystem.Tests.Integration.TypeBroadcastTest
{
    using Runtime.Connections;
    using UnityEngine;

    public class TypeBroadCasterTests : MonoBehaviour
    {
        private TypeDataBrodcaster brodcaster;
        
        public int counter = 10000;

        private void Start()
        {
            brodcaster = new TypeDataBrodcaster();
        }

        private void Update()
        {
            for (int i = 0; i < counter; i++) {
                //Profiler.BeginSample("TypeBroadCasterTests.PublishTest");
                PublishTest(i);
                //Profiler.EndSample();
            }
        }

        private void PublishTest(int index)
        {
            brodcaster.Publish(index);
        }
    }
}
