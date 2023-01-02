# dotnet-estimation
With this project, we want to deliver a lightweight open-source tool for scrum-based workflows in which new task complexities can be easily estimated by the team and archived for a possible retrospective.

## How to start the estimation application
Run the Application by executing the
``
docker-compose up
``
command in the root folder.

## How to setup the project to use a self-signed certificate for signing JSON Web Tokens
Please read the following guide to set up your development environment to use a given certificate for signing the generated JSON Web Tokens correctly.
Read the short introduction until row 15 and then choose one of the preferred options below.
I assume you already have a certificate. In case you do not, you can follow the following link to create one [Create a self-signed certificate](https://github.com/devonfw-forge/dotnet-estimation/blob/main/dotnet-estimation/dotnet-estimation/Templates/WebAPI/Devon4Net.Application.WebAPI/Certificates/HowToSetupJwtViaSelfSignedCa.txt)  or you just ask your projectmembers if there already exists one.

- If you're using **Visual Studio**, I recommend you to follow the steps beginning from row 42.
- Otherwise, I would recommend you to use the **Script**. The steps are explained from row 17 to 22.
- In case you prefer the **CLI**, read on from row 24 to 40.

[Guide to setup JWT generation signed by a given certificate in developement environment](https://github.com/devonfw-forge/dotnet-estimation/blob/main/dotnet-estimation/dotnet-estimation/Templates/WebAPI/Devon4Net.Application.WebAPI/Certificates/HowToSetupJwt.txt "Link")


## Routes
The estimation application's backend provides the following services and routes:


### Session Status
Provides the user with information about if a session is valid and the task which is currently estimated.

- CRUD: **GET**
- Route: */estimation/v1/session/{id}/status*
- Parameter
    - _id_: Session ID
- Returns ``statusResult``-Object, including
    - ``bool isValid``
    - ``TaskDto currentTask``
        - ``long Id``
        - ``string Title``
        - ``string Description``
        - ``string Url``
        - ``Status Status``
            - ``enum Status``

### Create Session
Enables a user to create a new estimation session.

- CRUD: **POST**
- Route: */estimation/v1/session/newSession*
- Returns ``Session``-Object, including
    - ``long Id``
    - ``string InviteToken``
    - ``DateTime ExpiresAt``
    - ``List<Task> Tasks``
    - ``List<User> Users``
    - ``bool isValid``

### Add User to Session
Enables a user to join an existing and valid estimation session.

- CRUD: **POST**
- Route: */estimation/v1/session/{sessionId:long}/{userId}/joinSession*
- Parameter:
    - _sessionId_: Session Id
    - _userId_: User Id
- Returns ``boolean``
    - ``true`` if successful
    - ``false`` if unsuccessful

### Add Estimation
Enables a user to provide an estimation during an existing and valid estimation session.

- CRUD: **POST**
- Route: */estimation/v1/session/{sessionId:long}/estimation*
- Parameter
    - _sessionId_: Session Id
    - _estimationDto_: Object including ``string VoteBy`` and ``int Complexity``
- Returns ``EstimationDto``-Object, including
    - ``string VoteBy``
    - ``int Complexity``

### Remove User from Session
Removes a user from an existing and valid estimation session.

- CRUD: **PUT**
- Route: */estimation/v1/session/{sessionId}/leaveSession/{userId}*
- Parameter
    - _sessionId_: Session Id
    - _userId_: User Id
- Returns ``boolean``
    - ``true`` if successful
    - ``false`` if unsuccessful

### Invalidate/Terminate Session
Enables a user to invalidate/terminate an existing and valid estimation session.

- CRUD: **PUT**
- Route: */estimation/v1/session/{id:long}/invalidate*
- Parameter
    - _id_: Session Id
- Returns ``boolean``
    - ``true`` if successful
    - ``false`` if unsuccessful




