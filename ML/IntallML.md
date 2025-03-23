# INSTALL ML
Notes taken from source: https://www.youtube.com/watch?v=bT3SV1SLqHA

## Step By Step
1. Install C++ redistibutable: https://unity-technologies.github.io/ml-agents/Installation/#windows-installing-pytorch
2. Install CUDA => 
   Version: https://unity-technologies.github.io/ml-agents/Installation/#windows-installing-pytorch
   Link: https://developer.nvidia.com/cuda-toolkit-archive
3. Install CUDNN
    Cuda Compatibility: https://docs.nvidia.com/deeplearning/cudnn/backend/latest/reference/support-matrix.html
    Link: https://developer.nvidia.com/cudnn-archive
4. Install Conda: https://docs.conda.io/projects/conda/en/stable/

-----------------------
5 - Run commands in Conda:
-----------------------
Some mentioned here: https://unity-technologies.github.io/ml-agents/Installation/#conda-python-setup

'conda create -n mlagents python=3.10.12'  
'conda activate mlagents' => Activate (later for deactivate: 'conda deactivate')  
Check numpy requirement: https://docs.unity3d.com/Packages/com.unity.ml-agents@3.0/changelog/CHANGELOG.html
'conda install numpy=1.23.5'
Copy Pytorch install command from https://unity-technologies.github.io/ml-agents/Installation/#windows-installing-pytorch
'pip3 install torch~=2.2.1 --index-url https://download.pytorch.org/whl/cu121'

Check it works
'python' then 'import torch' then 'import numpy' (no message feedback)
'print(torch.__version__)' then 'print(numpy.__version__)'
'exit()' to exit python
'clear' to clear the terminal 

-----------------------
6 - Get Source Files
-----------------------
Source from here: https://github.com/Unity-Technologies/ml-agents/releases/tag/release_22
Get Source Code
Extract close to unity projects folder (might rename it to ml-agents)

-----------------------

7. Go to file path => cd D:\U\ml-agents (try with D: if it doesn't go it right there)
8. Run commands from here: https://unity-technologies.github.io/ml-agents/Installation/#installing-mlagents
    'python -m pip install ./ml-agents-envs'
    'python -m pip install ./ml-agents'
    'mlagents-learn --help' to check all good
9. Leave shell open and go to unity
10. Male sure ML package is installed

START TRAINING
(in shell) ml-learn [yaml path] --run-id=run1
=> Press play in unity to start the process
