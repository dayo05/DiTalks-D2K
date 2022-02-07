# DiTalks-D2K
## Install guide
```
wget https://packages.microsoft.com/config/ubuntu/20.04/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
sudo dpkg -i packages-microsoft-prod.deb
rm packages-microsoft-prod.deb

sudo apt-get update; \
  sudo apt-get install -y apt-transport-https && \
  sudo apt-get update && \
  sudo apt-get install -y dotnet-sdk-6.0
  
sudo apt install git
git clone https://github.com/Iroom-gbs/DiTalks-D2K.git

cd DiTalks-D2K
dotnet build --configuration Release
cd DiTalks-D2K/bin/Release/net6.0/
echo "[[your-token]]" >> token.txt
nohup ./DiTalks-D2K &
