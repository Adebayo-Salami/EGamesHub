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
    [Migration("20201202184616_Err")]
    partial class Err
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

            modelBuilder.Entity("EGamesData.Models.BrainGameQuestion", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .UseIdentityColumn();

                    b.Property<int?>("AddedById")
                        .HasColumnType("int");

                    b.Property<string>("Answers")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("BrainGameCategory")
                        .HasColumnType("int");

                    b.Property<string>("Question")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("AddedById");

                    b.ToTable("BrainGameQuestions");
                });

            modelBuilder.Entity("EGamesData.Models.Challenge", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .UseIdentityColumn();

                    b.Property<double>("AmountToBeWon")
                        .HasColumnType("float");

                    b.Property<double>("AmountToStaked")
                        .HasColumnType("float");

                    b.Property<int?>("BingoGameId")
                        .HasColumnType("int");

                    b.Property<int?>("BrainGameCategory")
                        .HasColumnType("int");

                    b.Property<int?>("BrainGameQuestion1Id")
                        .HasColumnType("int");

                    b.Property<int?>("BrainGameQuestion2Id")
                        .HasColumnType("int");

                    b.Property<int?>("BrainGameQuestion3Id")
                        .HasColumnType("int");

                    b.Property<int?>("BrainGameQuestion4Id")
                        .HasColumnType("int");

                    b.Property<int?>("BrainGameQuestion5Id")
                        .HasColumnType("int");

                    b.Property<string>("ChallengeLink")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ChallengeName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("ChallengeStatus")
                        .HasColumnType("int");

                    b.Property<DateTime>("DateCreated")
                        .HasColumnType("datetime2");

                    b.Property<int?>("GameHostId")
                        .HasColumnType("int");

                    b.Property<int>("GameHostPoints")
                        .HasColumnType("int");

                    b.Property<bool>("GameHostSaysGameIsOngoing")
                        .HasColumnType("bit");

                    b.Property<string>("GameSummary")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("GameType")
                        .HasColumnType("int");

                    b.Property<bool>("IsChallengeEnded")
                        .HasColumnType("bit");

                    b.Property<bool>("IsChallengeStarted")
                        .HasColumnType("bit");

                    b.Property<bool>("IsGameHostDone")
                        .HasColumnType("bit");

                    b.Property<bool>("IsUserChallengedDone")
                        .HasColumnType("bit");

                    b.Property<bool>("IsUserJoined")
                        .HasColumnType("bit");

                    b.Property<DateTime>("TimeGameHostEnded")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("TimeUserChallengeEnded")
                        .HasColumnType("datetime2");

                    b.Property<int?>("UserChallengedId")
                        .HasColumnType("int");

                    b.Property<int>("UserChallengedPoints")
                        .HasColumnType("int");

                    b.Property<bool>("UserChallengedSaysGameIsOngoing")
                        .HasColumnType("bit");

                    b.Property<int?>("WinningUserId")
                        .HasColumnType("int");

                    b.Property<int?>("WordPuzzleId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("BingoGameId");

                    b.HasIndex("BrainGameQuestion1Id");

                    b.HasIndex("BrainGameQuestion2Id");

                    b.HasIndex("BrainGameQuestion3Id");

                    b.HasIndex("BrainGameQuestion4Id");

                    b.HasIndex("BrainGameQuestion5Id");

                    b.HasIndex("GameHostId");

                    b.HasIndex("UserChallengedId");

                    b.HasIndex("WinningUserId");

                    b.HasIndex("WordPuzzleId");

                    b.ToTable("Challenges");
                });

            modelBuilder.Entity("EGamesData.Models.GameHistory", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .UseIdentityColumn();

                    b.Property<double>("AmountSpent")
                        .HasColumnType("float");

                    b.Property<double>("AmountWon")
                        .HasColumnType("float");

                    b.Property<DateTime>("DatePlayed")
                        .HasColumnType("datetime2");

                    b.Property<int>("GameType")
                        .HasColumnType("int");

                    b.Property<string>("SelectedValues")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("UserId")
                        .HasColumnType("int");

                    b.Property<string>("WinningValues")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("GameHistories");
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

                    b.Property<string>("Narration")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("TransactionType")
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

                    b.Property<string>("AgentCode")
                        .HasColumnType("nvarchar(max)");

                    b.Property<double>("AmtUsedToPlayBrainGame")
                        .HasColumnType("float");

                    b.Property<double>("AmtUsedToPlayChallenge")
                        .HasColumnType("float");

                    b.Property<double>("AmtUsedToPlayWordPuzzle")
                        .HasColumnType("float");

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

                    b.Property<bool>("IsWithdrawing")
                        .HasColumnType("bit");

                    b.Property<DateTime>("LastLoginDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<double>("PendingWithdrawalAmount")
                        .HasColumnType("float");

                    b.Property<string>("ReferralCode")
                        .HasColumnType("nvarchar(max)");

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

            modelBuilder.Entity("EGamesData.Models.WordPuzzle", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .UseIdentityColumn();

                    b.Property<int?>("AddedById")
                        .HasColumnType("int");

                    b.Property<string>("Answers")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Explanation")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("AddedById");

                    b.ToTable("WordPuzzles");
                });

            modelBuilder.Entity("EGamesData.Models.BrainGameQuestion", b =>
                {
                    b.HasOne("EGamesData.Models.User", "AddedBy")
                        .WithMany()
                        .HasForeignKey("AddedById");

                    b.Navigation("AddedBy");
                });

            modelBuilder.Entity("EGamesData.Models.Challenge", b =>
                {
                    b.HasOne("EGamesData.Models.Bingo", "BingoGame")
                        .WithMany()
                        .HasForeignKey("BingoGameId");

                    b.HasOne("EGamesData.Models.BrainGameQuestion", "BrainGameQuestion1")
                        .WithMany()
                        .HasForeignKey("BrainGameQuestion1Id");

                    b.HasOne("EGamesData.Models.BrainGameQuestion", "BrainGameQuestion2")
                        .WithMany()
                        .HasForeignKey("BrainGameQuestion2Id");

                    b.HasOne("EGamesData.Models.BrainGameQuestion", "BrainGameQuestion3")
                        .WithMany()
                        .HasForeignKey("BrainGameQuestion3Id");

                    b.HasOne("EGamesData.Models.BrainGameQuestion", "BrainGameQuestion4")
                        .WithMany()
                        .HasForeignKey("BrainGameQuestion4Id");

                    b.HasOne("EGamesData.Models.BrainGameQuestion", "BrainGameQuestion5")
                        .WithMany()
                        .HasForeignKey("BrainGameQuestion5Id");

                    b.HasOne("EGamesData.Models.User", "GameHost")
                        .WithMany()
                        .HasForeignKey("GameHostId");

                    b.HasOne("EGamesData.Models.User", "UserChallenged")
                        .WithMany()
                        .HasForeignKey("UserChallengedId");

                    b.HasOne("EGamesData.Models.User", "WinningUser")
                        .WithMany()
                        .HasForeignKey("WinningUserId");

                    b.HasOne("EGamesData.Models.WordPuzzle", "WordPuzzle")
                        .WithMany()
                        .HasForeignKey("WordPuzzleId");

                    b.Navigation("BingoGame");

                    b.Navigation("BrainGameQuestion1");

                    b.Navigation("BrainGameQuestion2");

                    b.Navigation("BrainGameQuestion3");

                    b.Navigation("BrainGameQuestion4");

                    b.Navigation("BrainGameQuestion5");

                    b.Navigation("GameHost");

                    b.Navigation("UserChallenged");

                    b.Navigation("WinningUser");

                    b.Navigation("WordPuzzle");
                });

            modelBuilder.Entity("EGamesData.Models.GameHistory", b =>
                {
                    b.HasOne("EGamesData.Models.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId");

                    b.Navigation("User");
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

            modelBuilder.Entity("EGamesData.Models.WordPuzzle", b =>
                {
                    b.HasOne("EGamesData.Models.User", "AddedBy")
                        .WithMany()
                        .HasForeignKey("AddedById");

                    b.Navigation("AddedBy");
                });
#pragma warning restore 612, 618
        }
    }
}
