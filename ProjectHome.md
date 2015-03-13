URMAH (User & Role Manager And Handlers) is a user and role (ASP.NET Membership) management facility. Inspired by [ELMAH](http://elmah.googlecode.com), the popular Error Logging module for ASP.NET, URMAH is a pluggable [ASP.NET HttpHandler](http://msdn.microsoft.com/en-us/library/f3ff8w4a(VS.71).aspx), which means that it can be added dynamically to any ASP.NET application that relies on the ASP.NET Membership framework, by simply dropping the assembly to the bin folder and adding one line to the Web.config.

Once URMAH has been dropped into a web application, you can:
  * Search users
  * View their information
  * Add/remove roles to users

This is a great way to browse the application users without having to write a single line of code, re-compiling or even re-deploying the application.

_Note: this project is currently is an extremely early stage. For example, the Roles management hasn't been implemented yet (except for adding/remove roles to and from a user), nor has the User creation part._

So to start using URMAH today you need:

1. Drop a copy of Urmah.dll into your Web Application /bin folder

2. In your Web.config:

Add this line into you <system.web>/

&lt;httpHandlers&gt;

 if you're using IIS6

```
<add verb="*" path="urmah.axd" type="Urmah.UserAndRolePageFactory, Urmah" />
```

Add this line into your <system.webServer>/

&lt;handlers&gt;

 if you're using IIS7 integrated mode

```
<add name="URMAH" verb="*" path="urmah.axd" type="Urmah.UserAndRolePageFactory, Urmah" preCondition="integratedMode" />
```

3. Make sure you point (i.e the `path` attribute) that HttpHandler to a secure folder. You don't want half the world just browsing through your users ;)