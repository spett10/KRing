# KRing #

KRing is a simple command line application to store the passwords associated with the domains you use, in an encrypted format. It is written in C#.

### What is this repository for? ###

* This is a personal project for learning purposes. I do not recommend anyone to use the application for its intended purpose and expect it to be secure. If you use this application to encrypt your passwords and later find someone to have broken the application, I take no responsibility. Use at your own precaution. I simply needed the functionality and thus created the project for learning purposes. 

### How does KRing store passwords? ###

In laymen, we encrypt them.

In more detail:
-Your profile for the application has a master password. This password is checked upon registration, to see whether its "strong". This simply means that we employ a heuristic to make sure the password is not trivial. Thus, it must be of a certain length with a certain amount of special characters and digits. This does not mean that your password is unbreakable, it merely means that any bruteforce attack or rainbow table attack will take longer than if the password was trivial or weak.

-Your master password is not stored. Instead, we store a hash of your password, alongside the salt used for hashing it with the RFC2898 algorithm. This means we do not store your password, but we can still check at runtime whether you enter the correct password. (That is, the key used for decryption is only in memory at runtime, and is not hardcoded into the application, nor stored anywhere).

-Each run of the application will re-encrypt all stored passwords with semantically secure encryption, using AES with a 256 bit key. The key used for decryption is derived at runtime based on your profiles' password and a stored IV. The key is derived using the RFC2898 algorithm with 1000 iterations. The IVs used are derived in a similar manner, but from a different instance of RFC2898, independant of the password. 

-When you exit the application, a new IV is derived and used for encryption. This means that for each run of the application, the encryption almost certainly (except with negligible probability) will produce a different encryption file than the previous run of the application, although the passwords stored might be the same. 

-The semantically secure encryption also means that even if you are stupid enough to have a password equal to your domain,
i.e email@domain.com == password, then the resulting ciphertexts will still be different. 

-The passwords are decrypted at runtime upon request, using the SecureString class in C#. This means that the plaintext password will only be in memory of your computer a limited amount of time, though not at all time (this would be impossible).


### How do I get set up? ###

* Summary of set up
When the application starts, if the profile.txt file is empty (or corrupted/changed since last use), the application interprets you as being a new user. You will be requested a username as well as a password. The password will be 
tested for "strongness" and you must submit one that is strong enough. 

The application can only have a single user at a time. It is possible to delete everything from within the application, which means that the next run of the application will attempt to create a new user.



* Usage

There are limited functionalities after you log in. 

The main menu will present to you the following possible actions:

-V (view password): Shows you all the domains you have encrypted. Upon request the associated password will be decrypted and displayed at the console. 


-A (add password): You can add a domain and its associated password. Each keystroke of your typed in password is added to a SecureString class, which is encrypted memory. Your plaintext password will only be in memory prior to the application encrypting it and writing it to the underlying database, minimizing the time your plaintext password is in memory to a minimum. 

-U (update password): You can view your current domains, and choose to update one of the associated passwords, which proceeds as add password does. 

-D (delete password): You can choose to delete a domain and its stored password. They are then purged from memory and are thus not encrypted and written back to the underlying database upon logout. 


-X (delete user): Your profile and all stored passwords and domains are deleted. Cannot be undone.


-L (logout): logs you out. The application reencrypts all stored password from encrypted memory to an underlying database with semantically secure encryption.