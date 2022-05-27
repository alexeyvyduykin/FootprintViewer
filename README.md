## FootprintViewer


## Quick start

### 1. Определение установленных версий пакетов SDK для .NET:

```sh
dotnet --list-sdks
```

### 2. Установка пакета SDK 6.0 для .NET если отсутствует:

* Linux:

```sh
sudo apt-get update
sudo apt-get install -y apt-transport-https
sudo apt-get update
sudo apt-get install -y dotnet-sdk-6.0
```

### 3. Получение и запуск приложения FootprintViewer:

* Windows:

```sh
git clone https://github.com/alexeyvyduykin/FootprintViewer
cd ./FootprintViewer
powershell -ExecutionPolicy Unrestricted ./build.ps1 --target Run
```

* Linux:

```sh
git clone https://github.com/alexeyvyduykin/FootprintViewer
cd ./FootprintViewer 
bash ./build.sh --target Run
```
