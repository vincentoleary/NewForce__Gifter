﻿using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Gifter.Models;
using Gifter.Utils;

namespace Gifter.Repositories
{
    public class UserProfileRepository : BaseRepository, IUserProfileRepository
    {
        public UserProfileRepository(IConfiguration configuration) : base(configuration) { }

        public List<UserProfile> GetAll()
        {
            using (var conn = Connection)
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                          SELECT Id, Name, Email, ImageUrl, Bio, DateCreated
                            FROM UserProfile
                        ORDER BY DateCreated";

                    var reader = cmd.ExecuteReader();

                    var profiles = new List<UserProfile>();
                    while (reader.Read())
                    {
                        profiles.Add(new UserProfile()
                        {
                            Id = DbUtils.GetInt(reader, "Id"),
                            Name = DbUtils.GetString(reader, "Name"),
                            Email = DbUtils.GetString(reader, "Email"),
                            ImageUrl = DbUtils.GetString(reader, "ImageUrl"),
                            Bio = DbUtils.GetString(reader, "Bio"),
                            DateCreated = DbUtils.GetDateTime(reader, "DateCreated"),
                        });
                    }

                    reader.Close();

                    return profiles;
                }
            }
        }

        public UserProfile GetById(int id)
        {
            using (var conn = Connection)
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                          SELECT Id, Name, Email, ImageUrl, Bio, DateCreated
                            FROM UserProfile
                           WHERE Id = @Id";

                    DbUtils.AddParameter(cmd, "@Id", id);

                    var reader = cmd.ExecuteReader();

                    UserProfile profile = null;
                    if (reader.Read())
                    {
                        profile = new UserProfile()
                        {
                            Id = DbUtils.GetInt(reader, "Id"),
                            Name = DbUtils.GetString(reader, "Name"),
                            Email = DbUtils.GetString(reader, "Email"),
                            ImageUrl = DbUtils.GetString(reader, "ImageUrl"),
                            Bio = DbUtils.GetString(reader, "Bio"),
                            DateCreated = DbUtils.GetDateTime(reader, "DateCreated"),
                        };
                    }

                    reader.Close();

                    return profile;
                }
            }
        }

        public UserProfile GetByFirebaseUserId(string firebaseUserId)
        {
            using (var conn = Connection)
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        SELECT Id, FirebaseUserId, Name, Email, ImageUrl, Bio, DateCreated
                            FROM UserProfile
                    WHERE FirebaseUserId = @FirebaseuserId";

                    DbUtils.AddParameter(cmd, "@FirebaseUserId", firebaseUserId);

                    UserProfile userProfile = null;

                    var reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        userProfile = new UserProfile()
                        {
                            Id = DbUtils.GetInt(reader, "Id"),
                            FirebaseUserId = DbUtils.GetString(reader, "FirebaseUserId"),
                            Name = DbUtils.GetString(reader, "Name"),
                            Email = DbUtils.GetString(reader, "Email"),
                            ImageUrl = DbUtils.GetString(reader, "ImageUrl"),
                            Bio = DbUtils.GetString(reader, "Bio"),
                            DateCreated = DbUtils.GetDateTime(reader, "DateCreated"),
                        };
                    }
                    reader.Close();

                    return userProfile;
                }
            }
        }
        public UserProfile GetByIdWithPosts(int id)
        { return null; }

        public void Add(UserProfile profile)
        {
            using (var conn = Connection)
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        INSERT INTO UserProfile (FirebaseUserId, Name, Email, ImageUrl, Bio, DateCreated)
                        OUTPUT INSERTED.ID
                        VALUES (@FirebaseUserId, @Name, @Email, @ImageUrl, @Bio, @DateCreated)";

                    DbUtils.AddParameter(cmd, "@FirebaseUserId", profile.FirebaseUserId);
                    DbUtils.AddParameter(cmd, "@Name", profile.Name);
                    DbUtils.AddParameter(cmd, "@Email", profile.Email);
                    DbUtils.AddParameter(cmd, "@DateCreated", "6 / 21 / 2020 12:00:00 AM");
                    DbUtils.AddParameter(cmd, "@ImageUrl", profile.ImageUrl);
                    DbUtils.AddParameter(cmd, "@Bio", profile.Bio);

                    profile.Id = (int)cmd.ExecuteScalar();
                }
            }
        }

        public void Update(UserProfile profile)
        {
            using (var conn = Connection)
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        UPDATE UserProfile
                           SET Name = @Name,
                               Email = @Email,
                               DateCreated = @DateCreated,
                               ImageUrl = @ImageUrl,
                               Bio = @Bio
                         WHERE Id = @Id";

                    DbUtils.AddParameter(cmd, "@Name", profile.Name);
                    DbUtils.AddParameter(cmd, "@Email", profile.Email);
                    DbUtils.AddParameter(cmd, "@DateCreated", profile.DateCreated);
                    DbUtils.AddParameter(cmd, "@ImageUrl", profile.ImageUrl);
                    DbUtils.AddParameter(cmd, "@Bio", profile.Bio);
                    DbUtils.AddParameter(cmd, "@Id", profile.Id);

                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void Delete(int id)
        {
            using (var conn = Connection)
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "DELETE FROM UserProfile WHERE Id = @Id";
                    DbUtils.AddParameter(cmd, "@id", id);
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
