﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="FetchRelationship.aspx.cs" Inherits="WebService.Documentation.Relationships.FetchRelationship" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Metrocam - /v1/relationships/fetch?id={id}</title>
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
            GET /v1/relationships/fetch?id={id}<br />
            <br />
            <br />
            <div class="code">
                <i>Request: </i>
                <pre></pre>
            </div>
            <div class="code">
                <i>Response: </i>
                <pre>
{
  "ID": "4f5665ca5ad98505b850909c",
  "UserID": "4f74b2285ad9850a14ae04ac",
  "FollowingUserID": "4f84e925d47cd406d46faf6c",
  "CreatedDate": 0
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

