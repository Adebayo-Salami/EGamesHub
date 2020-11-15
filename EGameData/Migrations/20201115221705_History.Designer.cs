﻿// <auto-generated />
using System;
using EGamesData;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace EGamesData.Migrations
{
    [DbContext(typeof(EGamesContext))]
    [Migration("20201115221705_History")]
    partial class History
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .UseIdentityColumns()
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("ProductVersion", "5.0.0");

            modelBuilder.Entity("EGamesData.Models.Bingo", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .UseIdentityColumn();

                    b.Property<double>("AmountSpentToday")
                        .HasColumnType("float");

                    b.Property<string>("AvailableOptions")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsPlaying")
                        .HasColumnType("bit");

                    b.Property<string>("SelectedColor")
                        .HasColumnType("nvarchar(max)");

                    b.Property<double>("TotalAmountSpent")
                        .HasColumnType("float");

                    b.Property<double>("TotalAmountWon")
                        .HasColumnType("float");

                    b.HasKey("Id");

                    b.ToTable("Bingos");
                });

            modelBuilder.Entity("EGamesData.Models.Notification", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .UseIdentityColumn();

                    b.Property<DateTime>("DatePosted")
                        .HasColumnType("datetime2");

                    b.Property<string>("Message")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Notifications");
                });

            modelBuilder.Entity("EGamesData.Models.TransactionHistory", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .UseIdentityColumn();

                    b.Property<double>("AmountFunded")
                        .HasColumnType("float");

                    b.Property<DateTime>("DateFunded")
                        .HasColumnType("datetime2");

                    b.Property<int?>("FundedById")
                        .HasColumnType("int");

                    b.Property<int?>("UserFundedId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("FundedById");

                    b.HasIndex("UserFundedId");

                    b.ToTable("TransactionHistories");
                });

            modelBuilder.Entity("EGamesData.Models.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .UseIdentityColumn();

                    b.Property<string>("AccountNumber")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("AuthenticationToken")
                        .HasColumnType("nvarchar(max)");

                    b.Property<double>("Balance")
                        .HasColumnType("float");

                    b.Property<string>("BankName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("BingoProfileId")
                        .HasColumnType("int");

                    b.Property<DateTime>("DateJoined")
                        .HasColumnType("datetime2");

                    b.Property<string>("EmailAddress")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<double>("IsWithdrawing")
                        .HasColumnType("float");

                    b.Property<DateTime>("LastLoginDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<double>("PendingWithdrawalAmount")
                        .HasColumnType("float");

                    b.Property<double>("TotalAmountWon")
                        .HasColumnType("float");

                    b.Property<int>("TotalGamesPlayed")
                        .HasColumnType("int");

                    b.Property<double>("WithdrawableAmount")
                        .HasColumnType("float");

                    b.Property<bool>("isAdmin")
                        .HasColumnType("bit");

                    b.HasKey("Id");

                    b.HasIndex("BingoProfileId");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("EGamesData.Models.TransactionHistory", b =>
                {
                    b.HasOne("EGamesData.Models.User", "FundedBy")
                        .WithMany()
                        .HasForeignKey("FundedById");

                    b.HasOne("EGamesData.Models.User", "UserFunded")
                        .WithMany()
                        .HasForeignKey("UserFundedId");

                    b.Navigation("FundedBy");

                    b.Navigation("UserFunded");
                });

            modelBuilder.Entity("EGamesData.Models.User", b =>
                {
                    b.HasOne("EGamesData.Models.Bingo", "BingoProfile")
                        .WithMany()
                        .HasForeignKey("BingoProfileId");

                    b.Navigation("BingoProfile");
                });
#pragma warning restore 612, 618
        }
    }
}