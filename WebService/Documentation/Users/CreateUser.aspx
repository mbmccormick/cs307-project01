﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CreateUser.aspx.cs" Inherits="WebService.Documentation.Users.CreateUser" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Metrocam - /v1/users/create</title>
    <link rel="Stylesheet" href="/Stylesheet.css" />
</head>
<body>
    <form id="form1" runat="server">
    <div class="layout">
        <p class="title">
            Metrocam</p>
        <p class="tagline">
            Metrocam is a fun & quirky way to share your life with friends through a series
            of pictures. Snap a photo, then choose a filter to transform the look and feel of
            the shot into a memory to keep around forever.
        </p>
        <br />
        <div class="endpoint">
            POST /v1/users/create<br />
            <br />
            <br />
            <div class="code">
                <i>Request: </i>
                <pre>
{
  "Username":"libbypucc",
  "Name":"Libby",
  "EmailAddress":"libby.pucc@gmail.com",
  "Biography":"Just another Metrocammer!",
  "ProfilePictureID":"4f774fc1d47cd408fc5c1d3a",
  "Location":"Metrocam City"
}</pre>
            </div>
            <div class="code">
                <i>Response: </i>
                <pre>
{
  "ID":"4f876b12d47cd404c0b4e74e",
  "Username":"libbypucc",
  "Name":"Libby",
  "EmailAddress":"libby.pucc@gmail.com",
  "Biography":"Just another Metrocammer!",
  "ProfilePicture":{
    "ID":"4f774fc1d47cd408fc5c1d3a",
    "UserID":"4f5665ca5ad98505b850909c",
    "Caption":null,
    "Latitude":40.44698,
    "Longitude":-86.944189,
    "ViewCount":0,
     "LargeURL":"http:\/\/metrocam.blob.core.windows.net\/pictures\/4f5685ce5ad9850e545bb48d\/4d8345fa-f701-4be7-bb7b-61950d3cdaf7_l.jpg",
    "MediumURL":"http:\/\/metrocam.blob.core.windows.net\/pictures\/4f5685ce5ad9850e545bb48d\/4d8345fa-f701-4be7-bb7b-61950d3cdaf7_m.jpg",
    "SmallURL":"http:\/\/metrocam.blob.core.windows.net\/pictures\/4f5685ce5ad9850e545bb48d\/4d8345fa-f701-4be7-bb7b-61950d3cdaf7_s.jpg",
    "CreatedDate":1333219265
  },
  "Location":"Metrocam City",
  "CreatedDate":1334274834
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

