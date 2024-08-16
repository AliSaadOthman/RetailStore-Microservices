using IdentityModel;
using IdentityServer4.Models;
using IdentityServer4.Test;
using System.Security.Claims;

namespace IdentityServer
{
    public static class Config
    {
        public static IEnumerable<IdentityResource> GetIdentityResources() =>
            new List<IdentityResource>
            {
            new IdentityResources.OpenId(),
            new IdentityResources.Profile(),
            };

        public static IEnumerable<ApiResource> GetApiResources()
        {
            return new List<ApiResource>
        {
            new ApiResource("customer_api", "Customer Service API"),
            new ApiResource("product_api", "Product Service API"),
            new ApiResource("order_api", "Order Service API"),
            new ApiResource("payment_api", "Payment Service API"),
        };
        }

        public static IEnumerable<ApiScope> GetApiScopes()
        {
            return new List<ApiScope>
        {
            new ApiScope("customer.view", "View access to Customer Service"),
            new ApiScope("customer.edit", "Edit access to Customer Service"),
            new ApiScope("customer.remove", "Remove access to Customer Service"),
            new ApiScope("product.view", "View products in Product Service"),
            new ApiScope("product.edit", "Edit products in Product Service"),
            new ApiScope("product.remove", "Remove products in Product Service"),
            new ApiScope("user.profile", "Access user profile in User Service"),
        };
        }

        public static IEnumerable<Client> GetClients() =>
            new List<Client>
            {
            new Client
            {
                ClientId = "client",
                AllowedGrantTypes = GrantTypes.ClientCredentials,
                ClientSecrets =
                {
                    new Secret("secret".Sha256())
                },
                AllowedScopes = { "customer_api"}
            }
            };

        public static List<TestUser> GetUsers() =>
            new List<TestUser>
            {
            new TestUser
            {
                SubjectId = "1",
                Username = "customer",
                Password = "password",
            },
            new TestUser
            {
                SubjectId = "1",
                Username = "admin",
                Password = "password",
            }
            };
    }
}
