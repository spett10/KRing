#These files are required to build and run tests. 

New-Item -Path "KRingForm" -Name "Data" -ItemType "directory" -Force
New-Item -Path "KRingForm\Data" -Name "Debug" -ItemType "directory" -Force
New-Item -Path "KRingForm\Data" -Name "Release" -ItemType "directory" -Force

[String[]]$files = "profile.txt", "db.txt", "log.txt", "logIntegrity.txt"

foreach ($file in $files) {
    New-Item -Path "KRingForm\Data\*" -Name $file -ItemType "file" -Force    
}

New-Item -Path "UnitTests" -Name "Data" -ItemType "directory" -Force
New-Item -Path "UnitTests\Data" -Name "Debug" -ItemType "directory" -Force
New-Item -Path "UnitTests\Data" -Name "Release" -ItemType "directory" -Force

foreach ($file in $files) {
    New-Item -Path "UnitTests\Data\*" -Name $file -ItemType "file" -Force    
}
