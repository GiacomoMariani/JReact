using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace JReact
{
    public struct JECS_CurveBlob
    {
        public BlobArray<float> CurvePoints;
        private int NumberOfSamples;

        public float GetValue(float normalizedTime)
        {
            float sampleIndexFloat = (NumberOfSamples - 1) * normalizedTime;
            int   sampleIndex      = (int)math.floor(sampleIndexFloat);
            if (sampleIndex >= NumberOfSamples - 1) { return CurvePoints[NumberOfSamples - 1]; }

            float indexRemainder = sampleIndexFloat - sampleIndex;
            return math.lerp(CurvePoints[sampleIndex], CurvePoints[sampleIndex + 1], indexRemainder);
        }

        public static BlobAssetReference<JECS_CurveBlob> FromCurve(AnimationCurve curve, int numberOfSamples,
                                                                   Allocator      allocator = Allocator.Persistent)
        {
            using var               blobBuilder       = new BlobBuilder(Allocator.Temp);
            ref JECS_CurveBlob      curveBlob         = ref blobBuilder.ConstructRoot<JECS_CurveBlob>();
            BlobBuilderArray<float> sampledCurveArray = blobBuilder.Allocate(ref curveBlob.CurvePoints, numberOfSamples);
            curveBlob.NumberOfSamples = numberOfSamples;

            for (var i = 0; i < numberOfSamples; i++)
            {
                float samplePoint = (float)i / (numberOfSamples - 1);

                float sampleValue = curve.Evaluate(samplePoint);
                sampledCurveArray[i] = sampleValue;
            }

            BlobAssetReference<JECS_CurveBlob> blobAssetReference = blobBuilder.CreateBlobAssetReference<JECS_CurveBlob>(allocator);
            return blobAssetReference;
        }
    }
}
