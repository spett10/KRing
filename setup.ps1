#These files are required to build and run tests. 

New-Item -Path "KRingForm" -Name "Data" -ItemType "directory"
New-Item -Path "KRingForm\Data" -Name "Debug" -ItemType "directory"
New-Item -Path "KRingForm\Data" -Name "Release" -ItemType "directory"

New-Item -Path "KRingForm\Data\Debug" -Name "profile.txt" -ItemType "file"
New-Item -Path "KRingForm\Data\Debug" -Name "db.txt" -ItemType "file"
New-Item -Path "KRingForm\Data\Debug" -Name "logIntegrity.txt" -ItemType "file"

New-Item -Path "KRingForm\Data\Release" -Name "profile.txt" -ItemType "file"
New-Item -Path "KRingForm\Data\Release" -Name "db.txt" -ItemType "file"
New-Item -Path "KRingForm\Data\Release" -Name "logIntegrity.txt" -ItemType "file"

New-Item -Path "UnitTests" -Name "Data" -ItemType "directory"
New-Item -Path "UnitTests\Data" -Name "Debug" -ItemType "directory"
New-Item -Path "UnitTests\Data" -Name "Release" -ItemType "directory"

New-Item -Path "UnitTests\Data\Debug" -Name "profile.txt" -ItemType "file"
New-Item -Path "UnitTests\Data\Debug" -Name "db.txt" -ItemType "file"
New-Item -Path "UnitTests\Data\Debug" -Name "logIntegrity.txt" -ItemType "file"

New-Item -Path "UnitTests\Data\Release" -Name "profile.txt" -ItemType "file"
New-Item -Path "UnitTests\Data\Release" -Name "db.txt" -ItemType "file"
New-Item -Path "UnitTests\Data\Release" -Name "logIntegrity.txt" -ItemType "file"
