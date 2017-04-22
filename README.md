# KRing #

KRing is a simple application to store the passwords associated with the domains you use, in an encrypted format. It is written in C#. It exposes a cmd line interface as well as a simple forms application. 
It can derive cryptographically random passwords for you, and store them encrypted, meaning you never again have to use silly passwords such as "secret1234". 

### Who is KRing for? ###

This is a personal project for learning purposes. I do not recommend anyone to use the application for its intended purpose. Do not expect it to be secure, since it has not been penetration tested or peer reviewed by industry professionals. If you use this application, it is at your own risk, I take no responsibility. Use at your own precaution. 

### How does KRing store passwords? ###

In laymen, we encrypt them.

In more detail:

* Your profile for the application has a master password. This password is checked upon registration, to see whether its "strong". This simply means that we employ a heuristic to make sure the password is not trivial. Thus, it must be of a certain length with a certain amount of special characters and digits. This does not mean that your password is unbreakable, it merely means that any bruteforce attack or rainbow table attack will take longer than if the password was trivial or weak.

* Your master password is not stored. Instead, we store a hash of your password, alongside the salt used for hashing it with the scrypt algorithm. This means we do not store your password, but we can still check at runtime whether you enter the correct password. (That is, the key used for decryption is only in memory at runtime, and is not hardcoded into the application, nor stored anywhere). This means that the only person that can decrypt your passwords is YOU - and anyone else that knows your master password (hopefully noone). 

* Each run of the application will re-encrypt all stored passwords with semantically secure encryption, using AES with a 256 bit key. The key used for decryption is derived at runtime based on your profiles' password and a stored salt, and this salt is updated each time the passwords are reencrypted, meaning we reencrypt under a new key each time. The key is derived using the RFC2898 algorithm with 1000 iterations. The salt used is derived in a similar manner, but from a different instance of RFC2898, independant of the password. 

* The encryption to the underlying files are done with GCM. This ensures authenticated encryption, so if an active adversary manipulates the stored data, even as much as a single bit, this will be caught by the GCM decryption. This does mean that an
active adversary can destroy your stored passwords by manipulating them, but this is unavoidable. Further, if an active adversary has access to your memory and filesystem, you might have bigger problems. 

* When you exit the application, a new IV is derived and used for encryption. This means that for each run of the application, the encryption almost certainly (except with negligible probability) will produce a different encryption file than the previous run of the application, although the passwords stored might be the same. This means a passive adversary gains no information whether you store the same passwords or whether you have updated any. A passive adversary will, however, be able to see an upper bound on the amount of passwords you store,
but this is generally acceptable, since for all encryption, a passive adversary can observe an upper bound on the plaintext by observing the ciphertext.

* The semantically secure encryption also means that even if you are stupid enough to have a password equal to your domain,
i.e email@domain.com == password, then the resulting ciphertexts of the domain respectively the password will still be different. 

* The passwords are decrypted at runtime upon request, using the SecureString class in C# for storage until such a request. This means that the plaintext password will only be in memory of your computer for a limited and minimal amount of time.

This means that if other people get hold of your encrypted files from the application, as long as they do not have your master password they cannot interpret your stored, encrypted passwords. Further, we do not store your password anywhere, only a hash of it. We thus strongly recommend that you use a unique and very secure password for this application, and that you make a concentrated effort to remember it by heart and not write it down anywhere. 

### Summary of set up ###
When the application starts for the first time, the application interprets you as being a new user. You will be requested a username as well as a password. The password will be 
tested for "strongness" and you must submit one that is strong enough. This means it must have an upper case character, at least one number and special character, as well as be at least 8 characters long. This strength requirement
is due to the fact that your password is the only thing protecting the other passwords that are encrypted using the app, thus it must be strong for the overall solution to be somewhat secure. 

The application can only have a single user at a time. It is possible to delete everything from within the application, which means that the next run of the application will attempt to create a new user.

The Data folder holds the encrypted passwords as well as meta data that enables the application to maintain state between your logins. You can backup the data folder seperately from the app, and overwrite the existing data
folder in the future in order to restore the state of your application. Do NOT alter any of the files, this will most likely render the application unable to interpret the stored data in any meaningful way.

### Usage ###

The cmd line version is rather convoluted. The forms version is more userfriendly, and supports both password derivation and password storage. The former means that the app will
derive a series of cryptographically random bytes and store it for you, and the latter lets you input a password you want to have stored. We recommend you let the app derive random passwords for you, as opposed to the usual simpler, but easier to remember passwords we normally use. But since the application stores the passwords
encrypted, the fact that the password is random, and thus hard to remember, is a non issue. Simply log in to the app, view your password and use it to log in to another application. The only thing you have to remember is your master password.