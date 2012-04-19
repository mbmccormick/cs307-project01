﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Authenticate.aspx.cs" Inherits="WebService.Documentation.Authentication.Authenticate" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Metrocam - /v1/authenticate</title>
    <link rel="Stylesheet" href="/Stylesheet.css" />
</head>
<body>
    <form id="form1" runat="server">
    <div class="layout">
        <p class="title">
            Metrocam</p>
        <p class="tagline">
            Based on an open API and a powerful webservice, Metrocam boasts a beautiful user experience, powerfully integrated with your Windows Phone. It's that feeling of running into your best friend from college on 11th Avenue when she's wearing that old 'Seattle Seahawks' t-shirt. And making fun of her for it. Except anywhere. Anytime.  
        </p>
        <br />
        <div class="endpoint">
            POST /v1/authenticate<br />
            <br />
            <br />
            <div class="code">
                <i>Request: </i>
                <pre>
{
   "Username":"mbmccormick",
   "Password":"5f4dcc3b5aa765d61d8327deb882cf99"
}</pre>
            </div>
            <div class="code">
                <i>Response: </i>
                <pre>
{
  "User": {
    "Name": "Matt McCormick",
    "Username": "mbmccormick",
    "Location": "Purdue University",
    "CreatedDate": 0,
    "EmailAddress": "mbmccormick@gmail.com",
    "ID": "4f5665ca5ad98505b850909c",
    "Biography": "I am awesome at life.",
    "ProfilePicture": {
      "LargeURL": "http://metrocam.blob.core.windows.net/pictures/4f7124f25ad9850a042a5f2d/993e2713-155c-4f97-a475-adfc9d223a2c_l.jpg",
      "SmallURL": "http://metrocam.blob.core.windows.net/pictures/4f7124f25ad9850a042a5f2d/993e2713-155c-4f97-a475-adfc9d223a2c_s.jpg",
      "Longitude": -86.9443232194399,
      "ViewCount": 0,
      "CreatedDate": 1334063901,
      "Latitude": 40.4469600040842,
      "ID": "4f84331cd47cd409c4df24be",
      "MediumURL": "http://metrocam.blob.core.windows.net/pictures/4f7124f25ad9850a042a5f2d/993e2713-155c-4f97-a475-adfc9d223a2c_m.jpg",
      "Caption": "These are my keys. ",
      "UserID": "4f5665ca5ad98505b850909c"
    }
  },
  "Token": "a21f8a253abf45a08f68ad0292878710"
}</pre>
            </div>
        </div>
        <br />
        <br />
        <div class="footer">
            <div class="navigation">
                <a href="/default.aspx">Home</a> | <a href="/documentation/default.aspx">API Documentation</a>
                | <a href="/register.aspx">API Key Registration</a> | <a href="/resetpassword.aspx">
                    Forgot Password</a>
            </div>
            Copyright &copy; 2012 Metrocam. All rights reserved.
        </div>
    </div>
    </form>
</body>
</html>
