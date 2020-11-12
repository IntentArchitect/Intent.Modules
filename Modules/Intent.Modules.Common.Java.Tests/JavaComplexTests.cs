using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using Antlr4.Runtime;
using Intent.Modules.Common.Java.Weaving;
using Xunit;

namespace Intent.Modules.Common.Java.Tests
{
    public class JavaComplexTests
    {
        [Fact]
        public void TestJavaTime()
        {
            var code = @"
@IntentMerge
public class User implements Serializable {

    private static final long serialVersionUID = 1L;

    @Id
    @GeneratedValue(strategy = GenerationType.IDENTITY)
    private Long id;

    private String firstName;

    private String lastName;

    private String email;

    private boolean activated;

    private java.util.Date resetDate;


    public Long getId() {
        return id;
    }

    public void setId(Long id) {
        this.id = id;
    }

    public String getPassword() {
        return password;
    }

    public void setPassword(String password) {
        this.password = password;
    }

    public String getPassword() {
        return password;
    }

    public void setPassword(String password) {
        this.password = password;
    }

    public String getPassword() {
        return password;
    }

    public void setPassword(String password) {
        this.password = password;
    }

    public String getPassword() {
        return password;
    }

    public void setPassword(String password) {
        this.password = password;
    }

    public String getPassword() {
        return password;
    }

    public void setPassword(String password) {
        this.password = password;
    }

    @Override
    public boolean equals(Object o) {
        if (this == o) {
            return true;
        }
        if (o == null || getClass() != o.getClass()) {
            return false;
        }

        User user = (User) o;
        return !(user.getId() == null || getId() == null) && Objects.equals(getId(), user.getId());
    }

    @Override
    public int hashCode() {
        return Objects.hashCode(getId());
    }
}";
            var sw = Stopwatch.StartNew();
            var inputStream = new AntlrInputStream(new MemoryStream(Encoding.UTF8.GetBytes(code)));
            var javaLexer = new JavaLexer(inputStream);
            var tokens = new CommonTokenStream(javaLexer);
            var parser = new JavaParser(tokens);
            // Parsing succeeds but is wrong (e.g. doesn't pick up imports properly) when this is enabled.
            //parser.Interpreter.PredictionMode = PredictionMode.SLL; // Performance enhancement
            parser.compilationUnit();
            var elapsed = sw.ElapsedMilliseconds;
            sw.Stop();
        }

        [Fact]
        public void TestJava9Time()
        {
            var code = @"
@IntentMerge
public class User implements Serializable {

    private static final long serialVersionUID = 1L;

    @Id
    @GeneratedValue(strategy = GenerationType.IDENTITY)
    private Long id;

    private String firstName;

    private String lastName;

    private String email;

    private boolean activated;

    private java.util.Date resetDate;


    public Long getId() {
        return id;
    }

    public void setId(Long id) {
        this.id = id;
    }

    public String getPassword() {
        return password;
    }

    public void setPassword(String password) {
        this.password = password;
    }

    public String getPassword() {
        return password;
    }

    public void setPassword(String password) {
        this.password = password;
    }

    public String getPassword() {
        return password;
    }

    public void setPassword(String password) {
        this.password = password;
    }

    public String getPassword() {
        return password;
    }

    public void setPassword(String password) {
        this.password = password;
    }

    public String getPassword() {
        return password;
    }

    public void setPassword(String password) {
        this.password = password;
    }

    @Override
    public boolean equals(Object o) {
        if (this == o) {
            return true;
        }
        if (o == null || getClass() != o.getClass()) {
            return false;
        }

        User user = (User) o;
        return !(user.getId() == null || getId() == null) && Objects.equals(getId(), user.getId());
    }

    @Override
    public int hashCode() {
        return Objects.hashCode(getId());
    }
}";
            var sw = Stopwatch.StartNew();
            var inputStream = new AntlrInputStream(new MemoryStream(Encoding.UTF8.GetBytes(code)));
            var javaLexer = new Java9Lexer(inputStream);
            var tokens = new CommonTokenStream(javaLexer);
            var parser = new Java9Parser(tokens);
            // Parsing succeeds but is wrong (e.g. doesn't pick up imports properly) when this is enabled.
            //parser.Interpreter.PredictionMode = PredictionMode.SLL; // Performance enhancement
            parser.compilationUnit();
            var elapsed = sw.ElapsedMilliseconds;
            sw.Stop();
        }

        [Fact]
        public void MergesDuplicateMembers()
        {
            var merger = new JavaWeavingMerger();
            var result = merger.Merge(
                existingContent: @"
@IntentMerge
public class User {}", 
                outputContent: @"
@IntentMerge
public class User implements Serializable {

    private static final long serialVersionUID = 1L;

    @Id
    @GeneratedValue(strategy = GenerationType.IDENTITY)
    private Long id;

    private String firstName;

    private String lastName;

    private String email;

    private boolean activated;

    private java.util.Date resetDate;


    public Long getId() {
        return id;
    }

    public void setId(Long id) {
        this.id = id;
    }

    public String getPassword() {
        return password;
    }

    public void setPassword(String password) {
        this.password = password;
    }

    public String getPassword() {
        return password;
    }

    public void setPassword(String password) {
        this.password = password;
    }

    public String getPassword() {
        return password;
    }

    public void setPassword(String password) {
        this.password = password;
    }

    public String getPassword() {
        return password;
    }

    public void setPassword(String password) {
        this.password = password;
    }

    public String getPassword() {
        return password;
    }

    public void setPassword(String password) {
        this.password = password;
    }

    @Override
    public boolean equals(Object o) {
        if (this == o) {
            return true;
        }
        if (o == null || getClass() != o.getClass()) {
            return false;
        }

        User user = (User) o;
        return !(user.getId() == null || getId() == null) && Objects.equals(getId(), user.getId());
    }

    @Override
    public int hashCode() {
        return Objects.hashCode(getId());
    }
}");
            Assert.Equal(@"
@IntentMerge
public class User implements Serializable {

    private static final long serialVersionUID = 1L;

    @Id
    @GeneratedValue(strategy = GenerationType.IDENTITY)
    private Long id;

    private String firstName;

    private String lastName;

    private String email;

    private boolean activated;

    private java.util.Date resetDate;


    public Long getId() {
        return id;
    }

    public void setId(Long id) {
        this.id = id;
    }

    public String getPassword() {
        return password;
    }

    public void setPassword(String password) {
        this.password = password;
    }

    @Override
    public boolean equals(Object o) {
        if (this == o) {
            return true;
        }
        if (o == null || getClass() != o.getClass()) {
            return false;
        }

        User user = (User) o;
        return !(user.getId() == null || getId() == null) && Objects.equals(getId(), user.getId());
    }

    @Override
    public int hashCode() {
        return Objects.hashCode(getId());
    }
}", result);
        }

        [Fact]
        public void InsertsNewUsingClausesAtTheRightPlace()
        {
            var merger = new JavaWeavingMerger();
            var result = merger.Merge(
                existingContent: @"
package root.src.main.java.com.vod_ms_infinity.subscription_customer.application.services.impl;

import lombok.AllArgsConstructor;
import lombok.extern.slf4j.Slf4j;
import org.springframework.stereotype.Service;
import org.springframework.transaction.annotation.Transactional;

import javax.persistence.EntityManager;
import javax.persistence.NoResultException;
import javax.persistence.TypedQuery;
import root.src.main.java.com.vod_ms_infinity.subscription_customer.application.models.CustomerRetrievalModel;
import root.src.main.java.com.vod_ms_infinity.subscription_customer.application.services.CustomerService;

@Service
@AllArgsConstructor
@Slf4j
public class CustomerServiceImpl implements CustomerService {
    @Override
    @Transactional(readOnly = true)
    public CustomerRetrievalModel getCustomerBySubscription(String msisdn) {
        throw new NotImplementedException(""Your implementation here..."");
    }
}", outputContent: @"
package com.vod_ms_infinity.subscription_customer.application.services.impl;

import lombok.AllArgsConstructor;
import lombok.extern.slf4j.Slf4j;
import org.springframework.stereotype.Service;
import org.springframework.transaction.annotation.Transactional;

import javax.persistence.EntityManager;
import javax.persistence.NoResultException;
import javax.persistence.TypedQuery;
import com.vod_ms_infinity.subscription_customer.application.models.CustomerRetrievalModel;
import com.vod_ms_infinity.subscription_customer.application.services.CustomerService;

@Service
@AllArgsConstructor
@Slf4j
public class CustomerServiceImpl implements CustomerService {
    @Override
    @Transactional(readOnly = true)
    public CustomerRetrievalModel getCustomerBySubscription(String msisdn) {
        throw new NotImplementedException(""Your implementation here..."");
    }
}");
            Assert.Equal(@"
package com.vod_ms_infinity.subscription_customer.application.services.impl;

import lombok.AllArgsConstructor;
import lombok.extern.slf4j.Slf4j;
import org.springframework.stereotype.Service;
import org.springframework.transaction.annotation.Transactional;

import javax.persistence.EntityManager;
import javax.persistence.NoResultException;
import javax.persistence.TypedQuery;
import root.src.main.java.com.vod_ms_infinity.subscription_customer.application.models.CustomerRetrievalModel;
import root.src.main.java.com.vod_ms_infinity.subscription_customer.application.services.CustomerService;
import com.vod_ms_infinity.subscription_customer.application.models.CustomerRetrievalModel;
import com.vod_ms_infinity.subscription_customer.application.services.CustomerService;

@Service
@AllArgsConstructor
@Slf4j
public class CustomerServiceImpl implements CustomerService {
    @Override
    @Transactional(readOnly = true)
    public CustomerRetrievalModel getCustomerBySubscription(String msisdn) {
        throw new NotImplementedException(""Your implementation here..."");
    }
}", result);
        }
    }
}