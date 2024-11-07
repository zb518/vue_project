interface SignInUser {
    Id: String,
    UserName: string,
    RealName?: String,
    SecurityStamp: String,
    Email?: String,
    Roles?: Array<String> | null
}